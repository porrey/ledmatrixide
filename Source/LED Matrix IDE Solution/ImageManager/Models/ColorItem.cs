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
using Windows.UI;

namespace ImageManager
{
	public struct ColorItem
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

		public ColorItemType ItemType { get; set; }

		public static implicit operator Color(ColorItem c)
		{
			return Color.FromArgb(c.A, c.R, c.G, c.B);
		}

		public static implicit operator ColorItem(Color c)
		{
			return new ColorItem() { A = c.A, R = c.R, B = c.B, G = c.G };
		}

		public static ColorItem FromColor(Color color, ColorItemType itemType)
		{
			ColorItem returnValue = color;

			returnValue.ItemType = itemType;

			return returnValue;
		}

		public override string ToString()
		{
			return $"A={this.A}, R={this.R}, G={this.G}, B={this.B}, {this.ItemType}";
		}
	}
}
