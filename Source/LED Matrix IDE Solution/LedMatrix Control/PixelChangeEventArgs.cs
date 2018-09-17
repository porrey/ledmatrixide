using Windows.System;
using Windows.UI.Xaml;

namespace LedMatrixControl
{
	public class PixelSelectedEventArgs : RoutedEventArgs
	{
		public PixelSelectedEventArgs(int row, int column, VirtualKeyModifiers modifiers)
		{
			this.Row = row;
			this.Column = column;
			this.KeyModifiers = modifiers;
		}

		public int Row { get; protected set; }
		public int Column { get; protected set; }
		public VirtualKeyModifiers KeyModifiers { get; protected set; }
	}
}
