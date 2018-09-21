using Windows.UI;

namespace ImageManager
{
	public static class SomeDecorator
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
			float a = (float)(alpha / 255.0);
			float oneminusalpha = 1 - a;
			return (byte)((color * a) + (oneminusalpha * background));
		}

		public static Color NormalBlendColor(this Color foregroundColor, Color backgroundColor)
		{
			Color returnValue;

			byte r = NormalBlendColor(foregroundColor.R, backgroundColor.R, foregroundColor.A);
			byte g = NormalBlendColor(foregroundColor.R, backgroundColor.R, foregroundColor.A);
			byte b = NormalBlendColor(foregroundColor.R, backgroundColor.R, foregroundColor.A);

			returnValue = Color.FromArgb(255, r, g, b);

			return returnValue;
		}
	}
}
