using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Windows.UI;

namespace ImageConverter
{
	public class ColorItem
	{
		public enum ColorItemType
		{
			Background,
			Pixel,
			Sand
		}

		public byte A { get; set; }
		public byte B { get; set; }
		public byte G { get; set; }
		public byte R { get; set; }

		public ColorItemType ItemType { get; set; } = ColorItemType.Pixel;

		public static implicit operator Color(ColorItem c)
		{
			return Color.FromArgb(c.A, c.R, c.G, c.B);
		}

		public static implicit operator ColorItem(Color c)
		{
			return new ColorItem() { A = c.A, R = c.R, B = c.B, G = c.G };
		}
	}

	public class ColorMatrix
	{
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
	}
}
