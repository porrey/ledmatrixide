# LED Matrix IDE
----------

# Table of Contents #
- Overview
- Opening the Source Code
- Using the Application
- How it Works
	- Background
	- Main Code
		- Initialization
		- Rendering
			- Drawing the Image
			- Drawing the Sand

# Overview #

This Universal Windows Application  provides and IDE to create images and C++ code for the Adafruit 64x64 LED matrix and the [LED Matrix Sand Toy](https://learn.adafruit.com/matrix-led-sand "LED Matrix Sand Toy"). This is a great 3D Printing project that uses the [Adafruit 64x64 RGB LED Matrix](https://www.adafruit.com/product/3649 "Adafruit 64x64 RGB LED Matrix") and the [Adafruit RGB Matrix Bonnet for Raspberry Pi](https://www.adafruit.com/product/3211 "Adafruit RGB Matrix Bonnet for Raspberry Pi").

The hardware device has a mode button that allows you to switch between several sample images. This application will allow you to add your own images.

# Opening the Source Code

To open the source code, download the zip file and extract it to your disk, or clone the repository. If you haven't already done so, download and install Visual Studio 2017 from [https://visualstudio.microsoft.com/](https://visualstudio.microsoft.com/). 

# Using the Application

Open, build and deploy the application. start the application either from your Start menu or debug it in Visual Studio.

![](https://github.com/porrey/ledmatrixide/raw/master/Images/Screenshots/ScreenShot01.png)

The first step in creating an image for the LED Sand Toy is to import an image. The application will re-size any image you import but I recommend prepping the image in a good editor. Remember to leave room for the sand. An image 40 pixels by 40 pixels works well.

When ready, click the **Open File** tool bar button and select your image.

![](https://github.com/porrey/ledmatrixide/raw/master/Images/Screenshots/ScreenShot02.png)

The application provides some simple editing features, but is not intended to be a full editor. Make any necessary tweaks to your image. If you make a mistake, there are **Undo** and **Redo** options.

Next add the sand. There are two options to add sand. First, you can have it added randomly by the device. In this case you specify the number a grains you would like generated. The second option is to specify the starting location and color of each grain.

Select the **Sand Mode** tool from the tool bar and select your foreground color. Draw the pixels directly on the matrix keeping in mind that clicking an area with an image pixel will replace that pixel. Undo works here as well so don't worry about making mistakes.

If you would like to display a background color on the matrix chose the **Background Color** command from the tool bar. Image pixels can have an alpha channel and will be blended with the background when the code runs on the device.

![](https://github.com/porrey/ledmatrixide/raw/master/Images/Screenshots/ScreenShot03.png)

I recommend saving the image at this point. The application will save the image as a TIFF file preserving the pixels, the background color you choose and the sand pixels (color and placement).

Once you have completed your image and saved it, build the code by choosing the **Build** command on the tool bar. Build will use the project name for the code file name so choose a name you like.

When you click build you will need to choose a folder to output the files into.

![](https://github.com/porrey/ledmatrixide/raw/master/Images/Screenshots/ScreenShot04.png)

Select your output folder and click **Build Here**.

![](https://github.com/porrey/ledmatrixide/raw/master/Images/Screenshots/ScreenShot05.png)

The build process will create an instructions file explaining how to build the code on your device.

# How it Works #

## Background ##
The code generated by this application is a tweaked version of the original code and works mainly the same way. The major difference are:

1.  The image array *([height][width])* is a full color array in ARGB pixel format. The C# code  always generates a 64 x 64 array but the C++ code doesn't assume the size and will center it on the matrix if the image is smaller.

2.  The mask that defines obstacles for the sand grains is an array *([height][width])* of bits (1 or 0). The C# code always generate a 64 x 64 array but the C++ code doesn't assume the size and will center it on the matrix if the image is smaller.

3.  The grains of sand are contained in an array *([n][3])* of x and y positions followed by a color in RGB pixel format.

## Main Code ##

There are two parts of the main code to focus on. 

### Initialization ###

The first section is the initialization section. In this section the device and main components of the card are created and initialized. The most important part is the sand. It is initialized in the following line:

    sand = new Adafruit_PixelDust(width, height, nGrains, 1, 64, false);

Once initialized, the **Adafruit_PixelDust** instance needs to know the obstacles for the sand. This is done by calling `sand->setPixel(x, y)` for each position on the matrix that is an obstacle (a point the sand grain cannot be). The C++ code will loop through the mask array and set each obstacle as shown below.

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

In the code above, **x1** and **y1** are offsets used to center the mask in the case where it is smaller than 64 by 64 pixels.

The next step in the initialization is defining where to put the sand grains when the application on the device starts. The UWP application provides two ways to define the starting position. The first is to place them randomly on the device at start-up. In this case you simply define the number of grains to start with. 

    if (USE_RANDOM_SAND)
    {
    	// ***
    	// *** Initialize random snowflake positions
    	// ***
    	sand->randomize();
    }
    else
    {
    	...
    }


The second manner in which sand grains are defined is by drawing them on the matrix canvas. In this case, the position portion of the sand array is used to set the initial position of each grain.

    if (USE_RANDOM_SAND)
    {
    	...
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


### Rendering ###

The second section of the code is where the rendering of the display takes place. This section starts with the `while(running)` statement.

Within this loop, the accelerometer is read, the sand grains are iterated (the physics is applied), the background is cleared (using the background color if specified), the image is drawn and then the sand pixels are drawn.

#### Drawing the Image ####

The image is drawn by reading the image array for each pixel, converting the color integer into it's ARGB components and then blending it with the background color as shown below.


    for(y = 0; y < IMAGE_HEIGHT; y++) 
    {
    	for(x = 0; x < IMAGE_WIDTH; x++) 
    	{
    		uint color = image_color[y][x];
    		
    		// ***
    		// *** Break the color into it's components
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

#### Drawing the Sand ####

The sand is drawn in a similar manner. For each grain defined, get it's current position from the library (remember they are moving around) and then draw it using a specified color on the matrix. If the sand grains were randomized, then a default yellowish color is used (r = 200, green = 200, blue = 100 or #c8c864). If the sand grains were drawn on the canvas then the color is taken from the array.

    if (!USE_RANDOM_SAND)
    {
    	for(i = 0; i < nGrains; i++) 
    	{
    		// ***
    		// *** Get the position of the grain.
    		// ***
    		sand->getPosition(i, &x, &y);
    		
    		// ***
    		// *** Get the color of the grain.
    		// ***
    		uint color = grains[i][2];
    		
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
    	for(i = 0; i < nGrains; i++) 
    	{
    		
    		sand->getPosition(i, &x, &y);
    		led_canvas_set_pixel(canvas, x, y, 200, 200, 100);
    	}
    }

All of this work in the loop is done to an off-screen canvas which is swapped out on each refresh (double buffering).

*Last updated: 9/24/2018 6:45:07 AM*