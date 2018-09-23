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
	public static class CreateImageDataDecorator
	{
		/// <summary>
		/// Convert the ColorMatrix to a BGRA array.
		/// </summary>
		/// <param name="height"></param>
		/// <param name="width"></param>
		/// <returns></returns>
		public static Task<byte[]> CreateImageDataAsync(this IColorMatrix sourceColorMatrix)
		{
			byte[] returnValue = new byte[sourceColorMatrix.Width * sourceColorMatrix.Height * 4];
			int index = 0;

			for (int row = 0; row < sourceColorMatrix.Height; row++)
			{
				for (int column = 0; column < sourceColorMatrix.Width; column++)
				{
					Color color = sourceColorMatrix.ColorItems[row, column];

					// ***
					// *** Remove background and sand pixels.
					// ***
					if (sourceColorMatrix.ColorItems[row, column].ItemType == ColorItem.ColorItemType.Background)
					{
						// ***
						// *** Clear the color for transparent pixels.
						// ***
						color.A = 0;
						color.R = 0;
						color.G = 0;
						color.B = 0;
					}
					else if(sourceColorMatrix.ColorItems[row, column].ItemType == ColorItem.ColorItemType.Sand)
					{
						// ***
						// *** Leave the and color in place
						// ***
						color.A = 0;
					}

					returnValue[index + 0] = color.B;
					returnValue[index + 1] = color.G;
					returnValue[index + 2] = color.R;
					returnValue[index + 3] = color.A;

					index += 4;
				}
			}

			return Task.FromResult(returnValue);
		}
	}
}
