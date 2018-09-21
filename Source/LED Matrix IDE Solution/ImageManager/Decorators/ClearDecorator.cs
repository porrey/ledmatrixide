using System.Threading.Tasks;
using Windows.UI;

namespace ImageManager
{
	public static class ClearDecorator
	{
		public static async Task Clear(this ColorMatrix sourceColorMatrix, Color color)
		{
			for (uint row = 0; row < sourceColorMatrix.Height; row++)
			{
				for (uint column = 0; column < sourceColorMatrix.Width; column++)
				{
					await sourceColorMatrix.SetItem(row, column, ColorItem.FromColor(color, ColorItem.ColorItemType.Background));
				}
			}
		}
	}
}
