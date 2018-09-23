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
using Windows.UI;

namespace Matrix
{
	public interface IColorMatrix
	{
		Color BackgroundColor { get; set; }
		ColorItem[,] ColorItems { get; }
		uint Height { get; }
		uint Width { get; }

		event EventHandler<PixelChangedEventArgs> PixelChanged;
		event EventHandler BackgroundChanged;

		Task<ColorItem> GetItem(uint row, uint column);
		Task SetItem(uint row, uint column, Color color, ColorItem.ColorItemType itemType);
		Task SetItem(uint row, uint column, ColorItem color);
	}
}