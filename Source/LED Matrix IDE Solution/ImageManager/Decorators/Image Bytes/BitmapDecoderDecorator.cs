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
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace ImageManager
{
	public static class BitmapDecoderDecorator
	{
		public static async Task<(byte[] decodedBytes, uint newWidth, uint newHeight)> ResizeImageAsync(this BitmapDecoder decoder, uint maximumWidth, uint maximumHeight)
		{
			(byte[] decodedBytes, uint newWidth, uint newHeight) returnValue = (null, 0, 0);

			if (decoder.PixelHeight > maximumHeight || decoder.PixelWidth > maximumWidth)
			{
				using (InMemoryRandomAccessStream resizedStream = new InMemoryRandomAccessStream())
				{
					BitmapEncoder encoder = await BitmapEncoder.CreateForTranscodingAsync(resizedStream, decoder);

					double widthRatio = (double)maximumWidth / decoder.PixelWidth;
					double heightRatio = (double)maximumHeight / decoder.PixelHeight;

					double scaleRatio = Math.Min(widthRatio, heightRatio);

					if (maximumWidth == 0)
					{
						scaleRatio = heightRatio;
					}

					if (maximumHeight == 0)
					{
						scaleRatio = widthRatio;
					}

					returnValue.newHeight = (uint)Math.Floor(decoder.PixelHeight * scaleRatio);
					returnValue.newWidth = (uint)Math.Floor(decoder.PixelWidth * scaleRatio);

					encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Linear;
					encoder.BitmapTransform.ScaledHeight = returnValue.newHeight;
					encoder.BitmapTransform.ScaledWidth = returnValue.newWidth;

					await encoder.FlushAsync();
					resizedStream.Seek(0);

					byte[] resizedEncodedBytes = new byte[resizedStream.Size];
					await resizedStream.ReadAsync(resizedEncodedBytes.AsBuffer(), (uint)resizedStream.Size, InputStreamOptions.None);

					using (MemoryStream memoryStream = new MemoryStream(resizedEncodedBytes))
					{
						using (IRandomAccessStream imageStream = memoryStream.AsRandomAccessStream())
						{
							BitmapDecoder decoder2 = await BitmapDecoder.CreateAsync(imageStream);
							PixelDataProvider data = await decoder2.GetPixelDataAsync();
							returnValue.decodedBytes = data.DetachPixelData();
						}
					}
				}
			}

			return returnValue;
		}
	}
}
