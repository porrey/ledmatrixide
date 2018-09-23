using System;
using System.Text.RegularExpressions;
using Windows.UI;

namespace ImageManager
{
	public static class HexColorDecorator
	{
		public static string ToHexInt(this Color color)
		{
			return $"0x{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
		}

		public static string ToHexString(this Color color)
		{
			return $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
		}

		public static Color ToColor(this string hex)
		{
			Color color = new Color();

			if (hex.IsHexColor())
			{
				color.A = hex.Length == 9 ? (byte)(Convert.ToUInt32(hex.Substring(1, 2), 16)) : (byte)255;
				color.R = (byte)(Convert.ToUInt32(hex.Substring(hex.Length == 9 ? 3 : 1, 2), 16));
				color.G = (byte)(Convert.ToUInt32(hex.Substring(hex.Length == 9 ? 5 : 3, 2), 16));
				color.B = (byte)(Convert.ToUInt32(hex.Substring(hex.Length == 9 ? 7 : 5, 2), 16));
			}
			else
			{
				throw new ArgumentOutOfRangeException($"'{hex}' is not a valid color.");
			}

			return color;
		}

		public static bool IsHexColor(this string hex)
		{
			Regex regex = new Regex("^#(([0-9a-fA-F]{2}){3,4})$");
			return regex.IsMatch(hex);
		}
	}
}
