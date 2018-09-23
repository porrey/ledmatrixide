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
using System.Threading.Tasks;
using Matrix;
using Windows.UI;

namespace ImageManager
{
	public static class ReplaceColorDecorator
	{
		public static async Task ReplaceColorAsync(this IColorMatrix sourceColorMatrix, ColorItem oldColorItem, Color newColor, bool convertToBackground = false)
		{
			Color oldColor = oldColorItem;

			for (uint row = 0; row < sourceColorMatrix.Height; row++)
			{
				for (uint column = 0; column < sourceColorMatrix.Width; column++)
				{
					if (sourceColorMatrix.ColorItems[row, column] == oldColor &&
						sourceColorMatrix.ColorItems[row, column].ItemType == oldColorItem.ItemType)
					{
						ColorItem newItem = new ColorItem()
						{
							A = newColor.A,
							R = newColor.R,
							G = newColor.G,
							B = newColor.B,
							ItemType = convertToBackground ? ColorItem.ColorItemType.Background : sourceColorMatrix.ColorItems[row, column].ItemType
						};

						await sourceColorMatrix.SetItem(row, column, newItem);
					}
				}
			}
		}
	}
}
