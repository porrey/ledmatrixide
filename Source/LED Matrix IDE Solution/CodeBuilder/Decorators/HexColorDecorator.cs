using Windows.UI;

namespace CodeBuilder.Decorators
{
	public static class HexColorDecorator
	{
		public static string ColorToHex(this Color color)
		{
			return $"0x{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
		}
	}
}
