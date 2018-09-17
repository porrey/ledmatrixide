﻿using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;

namespace ImageConverter
{
	public class ImageFile
	{
		public ImageFile(StorageFile file, uint maximumHeight, uint maximumWidth)
		{
			this.File = file;
			this.MaximumHeight = maximumHeight;
			this.MaximumWidth = maximumWidth;
		}

		protected StorageFile File { get; set; }
		protected uint MaximumHeight { get; set; }
		protected uint MaximumWidth { get; set; }

		public async Task<ColorMatrix> Load()
		{
			ColorMatrix returnValue = new ColorMatrix(this.MaximumHeight, this.MaximumWidth);

			Stream imagestream = await this.File.OpenStreamForReadAsync();
			BitmapDecoder dec = await BitmapDecoder.CreateAsync(imagestream.AsRandomAccessStream());
			PixelDataProvider data = await dec.GetPixelDataAsync();
			byte[] bytes = data.DetachPixelData();

			if (dec.OrientedPixelWidth <= this.MaximumWidth && dec.OrientedPixelHeight <= this.MaximumHeight)
			{
				int startColumn = (int)((this.MaximumWidth - dec.OrientedPixelWidth) / 2.0);
				int startRow = (int)((this.MaximumHeight - dec.OrientedPixelHeight) / 2.0);

				for (int row = 0; row < dec.PixelHeight; row++)
				{
					for (int column = 0; column < dec.PixelWidth; column++)
					{
						Color color = this.GetPixel(bytes, row, column, dec.OrientedPixelWidth, dec.OrientedPixelHeight);

						if (color.A > 0)
						{
							returnValue.Colors[row + startRow, column + startColumn] = color;
						}
					}
				}
			}

			return returnValue;
		}

		public async Task<bool> Save(ColorMatrix colorMatrix)
		{
			bool returnValue = false;
		 
			byte[] data = await this.CreateImageData(this.MaximumHeight, this.MaximumWidth, colorMatrix);
			await this.CreateImage(this.MaximumHeight, this.MaximumWidth, data, this.File);
			returnValue = true;

			return returnValue;
		}

		protected async Task CreateImage(uint height, uint width, byte[] data, StorageFile storageFile)
		{
			using (IRandomAccessStream stream = await storageFile.OpenAsync(FileAccessMode.ReadWrite))
			{
				BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
				encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, width, height, 96, 96, data);
				await encoder.FlushAsync();
			}
		}

		protected Color GetPixel(byte[] pixels, int row, int column, uint width, uint height)
		{
			int index = (row * (int)width + column) * 4;

			byte b = pixels[index + 0];
			byte g = pixels[index + 1];
			byte r = pixels[index + 2];
			byte a = pixels[index + 3];

			return Color.FromArgb(a, r, g, b);
		}

		protected Task<byte[]> CreateImageData(uint height, uint width, ColorMatrix colorMatrix)
		{
			byte[] returnValue = new byte[height * width * 4];
			int index = 0;

			for (int row = 0; row < height; row++)
			{
				for (int column = 0; column < width; column++)
				{
					returnValue[index + 0] = colorMatrix.Colors[row, column].B;
					returnValue[index + 1] = colorMatrix.Colors[row, column].G;
					returnValue[index + 2] = colorMatrix.Colors[row, column].R;
					returnValue[index + 3] = colorMatrix.Colors[row, column].A;
					index += 4;
				}
			}

			return Task.FromResult(returnValue);
		}
	}
}