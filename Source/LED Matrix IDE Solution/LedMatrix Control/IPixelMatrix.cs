using System;
using System.Threading.Tasks;
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
		Task ClearAsync();
		Task<Color> GetPixelAsync(uint row, uint column);
		Task SetPixelAsync(uint row, uint column, Color backgroundColor);
		Task SetPixelAsync(uint row, uint column, Color backgroundColor, Color borderColor);
	}
}