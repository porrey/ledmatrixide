// Copyright © 2018 Daniel Porrey. All Rights Reserved.
//
// This file is part of the LED Matrix IDE Solution project.
// 
// The LED Matrix IDE Solution is free software: you can redistribute it
// and/or modify it under the terms of the GNU General Public License as
// published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
// 
// The LED Matrix IDE Solution is distributed in the hope that it will
// be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with the LED Matrix IDE Solution. If not, 
// see http://www.gnu.org/licenses/.
//
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Windows.UI;

namespace Matrix
{
	public class ColorMatrix : IColorMatrix
	{
		public event EventHandler<PixelChangedEventArgs> PixelChanged = null;
		public event EventHandler BackgroundChanged = null;

		public ColorMatrix(uint width, uint height)
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

		private Color _backgroundColor = Colors.Black;
		public Color BackgroundColor
		{
			get
			{
				return _backgroundColor;
			}
			set
			{
				_backgroundColor = value;
				this.OnBackgroundChanged();
			}
		}

		public Task SetItem(uint row, uint column, Color color, ColorItem.ColorItemType itemType)
		{
			ColorItem colorItem = color;
			colorItem.ItemType = itemType;

			this.ColorItems[row, column] = colorItem;
			this.OnPixelChanged(new PixelChangedEventArgs(row, column, colorItem, this.BackgroundColor));
			return Task.FromResult(0);
		}

		public Task SetItem(uint row, uint column, ColorItem color)
		{
			this.ColorItems[row, column] = color;
			this.OnPixelChanged(new PixelChangedEventArgs(row, column, color, this.BackgroundColor));
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

		protected virtual void OnBackgroundChanged()
		{
			if (this.BackgroundChanged != null)
			{
				this.BackgroundChanged.Invoke(this, new EventArgs());
			}
		}
	}
}
