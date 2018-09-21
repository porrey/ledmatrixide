using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI;

namespace ImageManager
{
	public static class GetPaletteDecorator
	{
		public static Task<IList<ColorItem>> GetPaletteAsync(this ColorMatrix sourceColorMatrix)
		{
			IList<ColorItem> colors = new List<ColorItem>();

			for (int row = 0; row < sourceColorMatrix.Height; row++)
			{
				for (int column = 0; column < sourceColorMatrix.Width; column++)
				{
					ColorItem color = sourceColorMatrix.ColorItems[row, column];

					if (!colors.Contains(color))
					{
						colors.Add(color);
					}
				}
			}

			return Task.FromResult(colors);
		}

	}
}
