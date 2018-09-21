using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI;

namespace ImageManager
{
	public static class LoadImageDecorator
	{
		public static async Task LoadAsync(this ColorMatrix sourceColorMatrix, StorageFile file, uint maximumHeight, uint maximumWidth)
		{
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
							Color color = await bytes.GetPixelAsync(row, column, width, height);
							await sourceColorMatrix.SetItem(row + startRow, column + startColumn, color, color.A > 0 ? ColorItem.ColorItemType.Pixel : ColorItem.ColorItemType.Background);
						}
					}
				}
				else
				{
					throw new BadImageFormatException();
				}
			}
		}
	}
}
