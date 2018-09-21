using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Windows.UI;

namespace ImageManager
{
	public class ColorMatrix
	{
		public event EventHandler<PixelChangedEventArgs> PixelChanged = null;

		public ColorMatrix(uint height, uint width)
		{
			this.Height = height;
			this.Width = width;
			this.ColorItems = new ColorItem[this.Height, this.Width];
		}

		[JsonConstructor]
		protected ColorMatrix(uint height, uint width, ColorItem[,] colorItems)
		{
			this.Height = height;
			this.Width = width;
			this.ColorItems = colorItems;
		}

		public uint Height { get; protected set; }
		public uint Width { get; protected set; }
		public ColorItem[,] ColorItems { get; protected set; }

		public Task SetItem(uint row, uint column, Color color, ColorItem.ColorItemType itemType)
		{
			ColorItem oldItem = this.ColorItems[row, column];
			ColorItem newItem = color;
			newItem.ItemType = itemType;

			this.ColorItems[row, column] = newItem;
			this.OnPixelChanged(new PixelChangedEventArgs(row, column, oldItem, newItem));
			return Task.FromResult(0);
		}

		public Task SetItem(uint row, uint column, ColorItem color)
		{
			ColorItem oldItem = this.ColorItems[row, column];
			this.ColorItems[row, column] = color;
			this.OnPixelChanged(new PixelChangedEventArgs(row, column, oldItem, color));
			return Task.FromResult(0);
		}

		public Task<ColorItem> GetItem(uint row, uint column)
		{
			return Task.FromResult(this.ColorItems[row, column]);
		}

		protected virtual void OnPixelChanged(PixelChangedEventArgs e)
		{
			if (this.PixelChanged != null)
			{
				this.PixelChanged.Invoke(this, e);
			}
		}
	}
}
