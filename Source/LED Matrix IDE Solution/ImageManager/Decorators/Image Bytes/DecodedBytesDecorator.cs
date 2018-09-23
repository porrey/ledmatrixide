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
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;

namespace ImageManager
{
	public static class DecodedBytesDecorator
	{
		public static Task<Color> GetPixelAsync(this byte[] decodedBytes, uint row, uint column, uint width, uint height)
		{
			uint index = (row * (uint)width + column) * 4;

			byte b = decodedBytes[index + 0];
			byte g = decodedBytes[index + 1];
			byte r = decodedBytes[index + 2];
			byte a = decodedBytes[index + 3];

			return Task.FromResult(Color.FromArgb(a, r, g, b));
		}

		public static async Task CreateImageAsync(this byte[] decodedBytes, uint height, uint width, StorageFile storageFile)
		{
			using (IRandomAccessStream stream = await storageFile.OpenAsync(FileAccessMode.ReadWrite))
			{
				BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.TiffEncoderId, stream);
				encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight, width, height, 96, 96, decodedBytes);
				await encoder.FlushAsync();
			}
		}
	}
}
