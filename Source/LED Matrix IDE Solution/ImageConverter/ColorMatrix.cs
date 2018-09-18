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
	}
}
