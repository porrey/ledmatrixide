using Windows.UI;

namespace ImageManager
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

		public ColorItemType ItemType { get; set; }

		public static implicit operator Color(ColorItem c)
		{
			return Color.FromArgb(c.A, c.R, c.G, c.B);
		}

		public static implicit operator ColorItem(Color c)
		{
			return new ColorItem() { A = c.A, R = c.R, B = c.B, G = c.G };
		}

		public static ColorItem FromColor(Color color, ColorItemType itemType)
		{
			ColorItem returnValue = color;

			returnValue.ItemType = itemType;

			return returnValue;
		}
	}
}
