/*!
 * @file bar-image.cpp
 *
 * This code is based on the Adafruit code found
 * at https://github.com/adafruit/Adafruit_PixelDust/tree/master/raspberry_pi
 * 
 * Example for Adafruit_PixelDust on Raspberry Pi.
 * Places an image Aas an obstacle in the middle 
 * of the playfield.
 *
 * REQUIRES rpi-rgb-led-matrix LIBRARY!
 * I2C MUST BE ENABLED using raspi-config!
 *
 */

// ***
// *** Arduino IDE sometimes aggressively builds subfolders
// ***
#ifndef ARDUINO

#include "Adafruit_PixelDust.h"
#include "led-matrix-c.h"
#include "lis3dh.h"
#include <signal.h>

// ***
// *** This file contains the image and obstacle bitmaps.
// ***
#include "bar-image.h"

struct RGBLedMatrix *matrix = NULL;
Adafruit_LIS3DH      lis3dh;
volatile bool        running = true;

// ***
// *** Background color (r,g,b)
// ***
#define BG_RED    0
#define BG_GREEN  0
#define BG_BLUE   0

// ***
// *** Accelerometer scaling (1-255). The accelerometer X, Y and Z values
// *** passed to the iterate() function will be multiplied by this value 
// *** and then divided by 256, e.g. pass 1 to divide accelerometer input
// *** by 256, 128 to divide by 2.
// ***
#define ACCELEROMETER_SCALING 1

// ***
// *** Particle elasticity (0-255). This determines the sand
// *** grains' "bounce". Higher numbers yield bouncier particles.
// ***
#define ELASTICITY 64

// ***
// *** If true, particles are sorted bottom-to-top when iterating. Sorting
// *** sometimes (not always) makes the physics less "Looney Tunes," as lower
// *** particles get out of the way of upper particles.  It can be computationally
// *** expensive if there's lots of grains, and isn't good if you're coloring 
// *** grains by index (because they're constantly reordering).
// ***
#define SORT_PARTICLES true

// ***
// *** Signal handler allows matrix to be properly deinitialized.
// ***
int sig[] = { SIGHUP,SIGINT,SIGQUIT,SIGABRT,SIGKILL,SIGBUS,SIGSEGV,SIGTERM };
#define N_SIGNALS (int)(sizeof sig / sizeof sig[0])

void irqHandler(int dummy) 
{
	if(matrix) 
	{
		led_matrix_delete(matrix);
		matrix = NULL;
	}
	
	for(int i = 0; i < N_SIGNALS; i++) 
	{
		signal(sig[i], NULL);
	}
	
	running = false;
}

uint8_t normalBlendColor(uint8_t color, uint8_t background, uint8_t alpha)
{
    float a = (float)(alpha / 255.0);
    float oneMinusAlpha = 1 - a;
    return ((color * a) + (oneMinusAlpha * background));
}

int main(int argc, char **argv) 
{
	struct RGBLedMatrixOptions options;
	struct LedCanvas          *canvas;
	int                        width, height, i, xx, yy, zz;
	Adafruit_PixelDust        *sand = NULL;
	dimension_t                x, y;

	for(i = 0; i < N_SIGNALS; i++) 
	{
		signal(sig[i], irqHandler); // ASAP!
	}
	
	// ***
	// *** Initialize LED matrix defaults
	// ***
	memset(&options, 0, sizeof(options));
	options.rows = 64;
	options.cols = 64;
	options.chain_length = 1;

	// ***
	// *** Parse command line input.  --led-help lists options!
	// ***
	matrix = led_matrix_create_from_options(&options, &argc, &argv);
	
	if(matrix != NULL)
	{
		// ***
		// *** Create offscreen canvas for double-buffered animation.
		// ***
		canvas = led_matrix_create_offscreen_canvas(matrix);
		led_canvas_get_size(canvas, &width, &height);
		fprintf(stderr, "Size: %dx%d. Hardware gpio mapping: %s\n", width, height, options.hardware_mapping);

		if(lis3dh.begin() == LIS3DH_OK) 
		{
			// ***
			// *** For this demo, the last argument to the PixelDust constructor
			// *** is set 'false' so the grains are not sorted each frame.
			// *** This is because the grains have specific colors by index
			// *** (sorting would mess that up).
			// ***
			sand = new Adafruit_PixelDust(width, height, NUM_GRAINS, ACCELEROMETER_SCALING, ELASTICITY, SORT_PARTICLES);
			
			if(sand->begin()) 
			{
				// ***
				// *** Set up the image bitmap obstacle in the PixelDust playfield
				// *** by centering it on the 64x64 grid.
				// ***
				int x1 = (width  - IMAGE_WIDTH ) / 2;
				int y1 = (height - IMAGE_HEIGHT) / 2;

				for(y = 0; y < IMAGE_HEIGHT; y++) 
				{
					for(x = 0; x < IMAGE_WIDTH; x++) 
					{
						uint8_t maskBit = image_mask[y][x];
						
						if (maskBit == 1)
						{
							sand->setPixel(x1 + x, y1 + y);
						}
					}
				}

				if (USE_RANDOM_SAND)
				{
					// ***
					// *** Initialize random snowflake positions
					// ***
					sand->randomize();
				}
				else
				{
					// ***
					// *** Set up initial sand coordinates
					// *** using the array in image.h.
					// ***
					for(i = 0; i < NUM_GRAINS; i++) 
					{
						uint8_t x = grains[i][0];
						uint8_t y = grains[i][1];
						sand->setPosition(i, x, y);
					}
				}
				
				while(running) 
				{
					// ***
					// *** Read accelerometer.
					// ***
					lis3dh.accelRead(&xx, &yy, &zz);

					// ***
					// *** Run one frame of the simulation.  Axis flip here
					// *** depends how the accelerometer is mounted relative
					// *** to the LED matrix.
					// ***
					sand->iterate(-xx, -yy, zz);

					// ***
					// *** led_canvas_fill() doesn't appear to work properly
					// *** with the --led-rgb-sequence option...so clear the
					// *** background manually with a bunch of set_pixel() calls.
					// ***
					for(y = 0; y < height; y++) 
					{
						for(x = 0; x < width; x++) 
						{
							led_canvas_set_pixel(canvas, x, y, BG_RED, BG_GREEN, BG_BLUE);
						}
					}
					
					// ***
					// *** Alpha-blend the image on the background...
					// ***
					for(y = 0; y < IMAGE_HEIGHT; y++) 
					{
						for(x = 0; x < IMAGE_WIDTH; x++) 
						{
							uint color = image_color[y][x];
							
							// ***
							// *** Break the color into its components
							// ***
							uint8_t a = (color >> 24);
							uint8_t r = normalBlendColor((color >> 16), BG_RED, a);
							uint8_t g = normalBlendColor((color >> 8), BG_GREEN, a);
							uint8_t b = normalBlendColor((color >> 0), BG_BLUE, a);

							// ***
							// *** Draw the image pixel.
							// ***
							led_canvas_set_pixel(canvas, x1 + x, y1 + y, r, g, b);
						}
					}

					// ***
					// *** Draw the sand on the canvas.
					// ***
					if (!USE_RANDOM_SAND)
					{
						for(i = 0; i < NUM_GRAINS; i++) 
						{
							// ***
							// *** Get the position of the grain.
							// ***
							sand->getPosition(i, &x, &y);
							
							// ***
							// *** Get the color of the grain.
							// ***
							uint color = grains[i][2];
							
							// ***
							// *** Break the color into its components
							// ***
							uint8_t r = (color >> 16);
							uint8_t g = (color >> 8);
							uint8_t b = (color >> 0);

							// ***
							// *** Draw the sand pixel.
							// ***
							led_canvas_set_pixel(canvas, x, y, r, b, g);
						}
					}
					else
					{
						for(i = 0; i < NUM_GRAINS; i++) 
						{
							
							sand->getPosition(i, &x, &y);
							led_canvas_set_pixel(canvas, x, y, 200, 200, 100);
						}
					}

					// ***
					// *** Update matrix contents on next vertical sync
					// *** and provide a new canvas for the next frame.
					// ***
					canvas = led_matrix_swap_on_vsync(matrix, canvas);
				}
			}
			else
			{
				puts("PixelDust init failed");
				return 3;
			}
		}
		else
		{
			puts("LIS3DH init failed");
			return 2;
		}
	}
	else
	{
		puts("Matrix not found!");
		return 1;
	}

	return 0;
}
#endif