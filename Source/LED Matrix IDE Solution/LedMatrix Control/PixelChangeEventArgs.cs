using Windows.System;
using Windows.UI.Xaml;

namespace LedMatrixControl
{
	public class PixelSelectedEventArgs : RoutedEventArgs
	{
		public PixelSelectedEventArgs(uint row, uint column, VirtualKeyModifiers modifiers)
		{
			this.Row = row;
			this.Column = column;
			this.KeyModifiers = modifiers;
		}

		public uint Row { get; protected set; }
		public uint Column { get; protected set; }
		public VirtualKeyModifiers KeyModifiers { get; protected set; }
	}
}
