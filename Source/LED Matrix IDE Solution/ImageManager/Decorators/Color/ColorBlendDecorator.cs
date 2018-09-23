using Windows.UI;

namespace ImageManager
{
	public static class ColorBlendDecorator
	{
		/// <summary>
		/// Blends a single color channel with a back ground color.
		/// </summary>
		/// <param name="color">A value specifying the color channel 
		/// value (red, green or blue).</param>
		/// <param name="background">A Byte value specifying the color channel 
		/// value (red, green or blue) of the background that the color will be 
		/// overlaid onto.</param>
		/// <param name="alpha">The alpha value to use for the color channel.</param>
		/// <returns></returns>
		public static byte NormalBlendColor(byte color, byte background, byte alpha)
		{
			Color c1 = Color.FromArgb(128, 255, 0, 0);

			float a = (float)(alpha / 255.0);
			float ia = 1 - a;

			return (byte)((a * color) + (ia * background));
		}

		/// <summary>
		/// Alpha blend the foreground over the background color and return the resulting color.
		/// </summary>
		/// <param name="foregroundColor"></param>
		/// <param name="backgroundColor"></param>
		/// <returns></returns>
		public static Color NormalBlendColor(this Color foregroundColor, Color backgroundColor)
		{
			Color returnValue;

			// ***
			// *** Apply the alpha value in each color.
			// ***
			Color fg = foregroundColor.ApplyAlpha();
			Color bg = backgroundColor.ApplyAlpha();

			// ***
			// *** Blend each channel.
			// ***
			byte r = NormalBlendColor(fg.R, bg.R, foregroundColor.A);
			byte g = NormalBlendColor(fg.G, bg.G, foregroundColor.A);
			byte b = NormalBlendColor(fg.B, bg.B, foregroundColor.A);

			// ***
			// *** Since these colors are going to end up being applied to an RGB LED
			// *** we want to express them in RGB values without an alpha value.
			// ***
			returnValue = Color.FromArgb(255, r, g, b);

			return returnValue;
		}

		/// <summary>
		/// Converts a color with an alpha channel value to the corresponding color (this
		/// would be the color applied to an RGB LED). This is the same as blending against
		/// a black background.
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>
		public static Color ApplyAlpha(this Color color)
		{
			float a = (float)(color.A / 255.0);
			return Color.FromArgb(255, (byte)(a * color.R), (byte)(a * color.G), (byte)(a * color.B));
		}
	}
}
