# LED Matrix IDE

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