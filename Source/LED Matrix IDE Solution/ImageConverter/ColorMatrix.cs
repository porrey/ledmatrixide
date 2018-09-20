using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Windows.UI;

namespace ImageConverter
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

		public Task<IList<ColorItem>> GetPaletteAsync()
		{
			IList<ColorItem> colors = new List<ColorItem>();

			for (int row = 0; row < this.Height; row++)
			{
				for (int column = 0; column < this.Width; column++)
				{
					ColorItem color = this.ColorItems[row, column];

					if (!colors.Contains(color))
					{
						colors.Add(color);
					}
				}
			}

			return Task.FromResult(colors);
		}

		public async Task RotateClockwiseAsync()
		{
			ColorMatrix me = await this.CloneAsync();

			for (uint row = 0; row < this.Height; row++)
			{
				for (uint column = 0; column < this.Width; column++)
				{
					this.ColorItems[row, column] = me.ColorItems[(this.Height - 1) - column, row];
				}
			}
		}

		public async Task RotateCounterClockwiseAsync()
		{
			ColorMatrix me = await this.CloneAsync();

			for (uint row = 0; row < this.Height; row++)
			{
				for (uint column = 0; column < this.Width; column++)
				{
					this.ColorItems[row, column] = me.ColorItems[column, (this.Height - 1) - row];
				}
			}
		}

		public async Task FlipVerticalAsync()
		{
			ColorMatrix me = await this.CloneAsync();

			for (uint row = 0; row < this.Height; row++)
			{
				for (uint column = 0; column < this.Width; column++)
				{
					this.ColorItems[row, column] = me.ColorItems[(this.Height - 1) - row, column];
				}
			}
		}

		public async Task FlipHorizontalAsync()
		{
			ColorMatrix me = await this.CloneAsync();

			for (uint row = 0; row < this.Height; row++)
			{
				for (uint column = 0; column < this.Width; column++)
				{
					this.ColorItems[row, column] = me.ColorItems[row, (this.Width - 1) - column];
				}
			}
		}

		public Task<ColorMatrix> CloneAsync()
		{
			ColorMatrix returnValue = new ColorMatrix(this.Height, this.Width);

			for (uint row = 0; row < this.Height; row++)
			{
				for (uint column = 0; column < this.Width; column++)
				{
					returnValue.ColorItems[column, row] = this.ColorItems[column, row];
				}
			}

			return Task.FromResult(returnValue);
		}

		/// <summary>
		/// Convert the ColorMatrix to a BGRA array.
		/// </summary>
		/// <param name="height"></param>
		/// <param name="width"></param>
		/// <returns></returns>
		public Task<byte[]> CreateImageDataAsync(uint height, uint width)
		{
			byte[] returnValue = new byte[height * width * 4];
			int index = 0;

			for (int row = 0; row < height; row++)
			{
				for (int column = 0; column < width; column++)
				{
					returnValue[index + 0] = this.ColorItems[row, column].B;
					returnValue[index + 1] = this.ColorItems[row, column].G;
					returnValue[index + 2] = this.ColorItems[row, column].R;
					returnValue[index + 3] = this.ColorItems[row, column].A;
					index += 4;
				}
			}

			return Task.FromResult(returnValue);
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
