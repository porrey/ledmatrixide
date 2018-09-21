using System.Threading.Tasks;
using ImageManager;
using LedMatrixControl;
using Windows.UI;

namespace LedMatrixIde.Decorators
{
	public static class ColorMatrixDecorator
	{
		//public static async Task SetColorMatrixAsync(this ColorMatrix colorMatrix, IPixelMatrix pixelMatrix)
		//{
		//	for (uint row = 0; row < pixelMatrix.RowCount; row++)
		//	{
		//		for (uint column = 0; column < pixelMatrix.ColumnCount; column++)
		//		{
		//			Color color = colorMatrix.ColorItems[row, column];

		//			if (color.A > 0)
		//			{
		//				await pixelMatrix.SetPixelAsync(row, column, color);
		//			}
		//		}
		//	}
		//}

		//public static async Task<ColorMatrix> GetColorMatrixAsync(this IPixelMatrix pixelMatrix)
		//{
		//	ColorMatrix returnValue = new ColorMatrix((uint)pixelMatrix.RowCount, (uint)pixelMatrix.ColumnCount);

		//	for (uint row = 0; row < pixelMatrix.RowCount; row++)
		//	{
		//		for (uint column = 0; column < pixelMatrix.ColumnCount; column++)
		//		{
		//			returnValue.ColorItems[row, column] = await pixelMatrix.GetPixelAsync(row, column);
		//		}
		//	}

		//	return returnValue;
		//}
	}
}
