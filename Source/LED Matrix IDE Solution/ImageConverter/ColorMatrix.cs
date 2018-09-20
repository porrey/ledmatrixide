using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Windows.UI;

namespace ImageConverter
{
	public class ColorMatrix
	{
		public ColorMatrix(uint height, uint width)
		{
			this.Height = height;
			this.Width = width;
			this.Colors = new Color[this.Height, this.Width];
		}

		[JsonConstructor]
		protected ColorMatrix(uint height, uint width, Color[,] colors)
		{
			this.Height = height;
			this.Width = width;
			this.Colors = colors;
		}

		public uint Height { get; protected set; }
		public uint Width { get; protected set; }
		public Color[,] Colors { get; protected set; }

		public Task<IList<Color>> GetPaletteAsync()
		{
			IList<Color> colors = new List<Color>();

			for (int row = 0; row < this.Height; row++)
			{
				for (int column = 0; column < this.Width; column++)
				{
					Color color = this.Colors[row, column];
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
					this.Colors[row, column] = me.Colors[(this.Height - 1) - column, row];
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
					this.Colors[row, column] = me.Colors[column, (this.Height - 1) - row];
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
					this.Colors[row, column] = me.Colors[(this.Height - 1) - row, column];
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
					this.Colors[row, column] = me.Colors[row, (this.Width - 1) - column];
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
					returnValue.Colors[column, row] = this.Colors[column, row];
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
					returnValue[index + 0] = this.Colors[row, column].B;
					returnValue[index + 1] = this.Colors[row, column].G;
					returnValue[index + 2] = this.Colors[row, column].R;
					returnValue[index + 3] = this.Colors[row, column].A;
					index += 4;
				}
			}

			return Task.FromResult(returnValue);
		}
	}
}
