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
		Task ClearMatrixAsync();
		Task<ColorMatrix> GetColorMatrixAsync();
		Task<Color> GetPixelAsync(int row, int column);
		Task ResetPixelAsync(int row, int column);
		Task SetColorMatrixAsync(ColorMatrix colorMatrix);
		Task SetPixelAsync(int row, int column, Color color);
	}
}