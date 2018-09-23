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
using System.IO;
using System.Threading.Tasks;
using Matrix;
using Project;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI;

namespace ImageManager
{
	public static class LoadImageDecorator
	{
		public static async Task<IMatrixProject> LoadAsync(this IColorMatrix sourceColorMatrix, StorageFile file, uint maximumHeight, uint maximumWidth)
		{
			IMatrixProject returnValue = new MatrixProject()
			{
				ColorMatrix = sourceColorMatrix
			};

			using (Stream imageStream = await file.OpenStreamForReadAsync())
			{
				BitmapDecoder decoder = await BitmapDecoder.CreateAsync(imageStream.AsRandomAccessStream());
				PixelDataProvider data = await decoder.GetPixelDataAsync();
				byte[] bytes = data.DetachPixelData();

				uint width = decoder.OrientedPixelWidth;
				uint height = decoder.OrientedPixelHeight;

				if (width > maximumWidth || height > maximumHeight)
				{
					(bytes, width, height) = await decoder.ResizeImageAsync(maximumHeight, maximumWidth);
				}

				if (width <= maximumWidth && height <= maximumHeight)
				{
					uint startColumn = (uint)((maximumWidth - width) / 2.0);
					uint startRow = (uint)((maximumHeight - height) / 2.0);

					for (uint row = 0; row < height; row++)
					{
						for (uint column = 0; column < width; column++)
						{
							// ***
							// *** Get the color from the image data.
							// ***
							Color color = await bytes.GetPixelAsync(row, column, width, height);

							// ***
							// *** The default color item type is pixel.
							// ***
							ColorItem.ColorItemType itemType = ColorItem.ColorItemType.Pixel;

							if (color.A == 0 && (color.R != 0 || color.G != 0 || color.B != 0))
							{
								// ***
								// *** An item with color and an alpha of 0 is a 
								// *** sand pixel.
								// ***
								itemType = ColorItem.ColorItemType.Sand;
								color.A = 255;
							}
							else if (color.A == 0)
							{
								// ***
								// *** An item without color (black) and an alpha
								// *** of 0 is a sand pixel.
								// ***
								itemType = ColorItem.ColorItemType.Background;
							}

							await sourceColorMatrix.SetItem(row + startRow, column + startColumn, color, itemType);
						}
					}

					// ***
					// *** Read the meta data.
					// ***
					await returnValue.RestoreImageMetaData(file);
				}
				else
				{
					throw new BadImageFormatException();
				}
			}

			return returnValue;
		}
	}
}
