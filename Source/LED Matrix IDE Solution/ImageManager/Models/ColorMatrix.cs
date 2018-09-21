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
