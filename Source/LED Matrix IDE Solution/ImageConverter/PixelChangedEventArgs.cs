using System;

namespace ImageConverter
{
	public class PixelChangedEventArgs : EventArgs
	{
		public PixelChangedEventArgs(uint row, uint column, ColorItem oldItem, ColorItem newItem)
		{
			this.Row = row;
			this.Column = column;
			this.OldItem = oldItem;
			this.NewItem = newItem;
		}

		public uint Row { get; protected set; }
		public uint Column { get; protected set; }
		public ColorItem OldItem { get; protected set; }
		public ColorItem NewItem { get; protected set; }
	}
}
