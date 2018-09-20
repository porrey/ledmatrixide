using System;
using Windows.UI;

namespace ImageConverter
{
	public struct ColorItem 
	{
		public enum ColorItemType
		{
			Background,
			Pixel,
			Sand
		}

		public byte A { get; set; }
		public byte B { get; set; }
		public byte G { get; set; }
		public byte R { get; set; }

		public ColorItemType ItemType { get; set; }// = ColorItemType.Pixel;

		public static implicit operator Color(ColorItem c)
		{
			Color returnValue = Color.FromArgb(0, 0, 0, 0);

			//if (c != null)
			//{
				returnValue = Color.FromArgb(c.A, c.R, c.G, c.B);
			//}

			return returnValue;
		}

		public static implicit operator ColorItem(Color c)
		{
			return new ColorItem() { A = c.A, R = c.R, B = c.B, G = c.G };
		}
	}
}
