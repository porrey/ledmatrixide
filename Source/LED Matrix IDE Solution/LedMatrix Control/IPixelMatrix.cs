using System;
using System.Threading.Tasks;
using ImageConverter;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace LedMatrixControl
{
	public interface IPixelMatrix
	{
		event EventHandler<PixelSelectedEventArgs> PixelSelected;
		Color DefaultBackgroundColor { get; }
		Color DefaultBorderColor { get; }
		int ColumnCount { get; set; }
		Brush PixelBackground { get; set; }
		Brush PixelBorder { get; set; }
		int RowCount { get; set; }
		Task ClearMatrix();
		Task<ColorMatrix> GetColorMatrix();
		Task<Color> GetPixel(int row, int column);
		Task ResetPixel(int row, int column);
		Task SetColorMatrix(ColorMatrix colorMatrix);
		Task SetPixel(int row, int column, Color color);
	}
}