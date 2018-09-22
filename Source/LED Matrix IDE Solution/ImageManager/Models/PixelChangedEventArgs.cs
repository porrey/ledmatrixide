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
using Windows.UI;

namespace ImageManager
{
	public class PixelChangedEventArgs : EventArgs
	{
		public PixelChangedEventArgs(uint row, uint column, ColorItem newItem, Color background)
		{
			this.Row = row;
			this.Column = column;
			this.NewItem = newItem;
			this.Background = background;
		}

		public uint Row { get; protected set; }
		public uint Column { get; protected set; }
		public ColorItem NewItem { get; protected set; }
		public Color Background { get; set; }
	}
}
