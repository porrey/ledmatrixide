using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
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

		public async Task<ColorMatrix> LoadAsync()
		{
			ColorMatrix returnValue = new ColorMatrix(this.MaximumHeight, this.MaximumWidth);

			using (Stream imageStream = await this.File.OpenStreamForReadAsync())
			{
				BitmapDecoder decoder = await BitmapDecoder.CreateAsync(imageStream.AsRandomAccessStream());
				PixelDataProvider data = await decoder.GetPixelDataAsync();
				byte[] bytes = data.DetachPixelData();

				uint width = decoder.OrientedPixelWidth;
				uint height = decoder.OrientedPixelHeight;

				if (width > this.MaximumWidth || height > this.MaximumHeight)
				{
					(bytes, width, height) = await decoder.ResizeImageAsync(this.MaximumHeight, this.MaximumWidth);
				}

				if (width <= this.MaximumWidth && height <= this.MaximumHeight)
				{
					int startColumn = (int)((this.MaximumWidth - width) / 2.0);
					int startRow = (int)((this.MaximumHeight - height) / 2.0);

					for (int row = 0; row < height; row++)
					{
						for (int column = 0; column < width; column++)
						{
							Color color = await bytes.GetPixelAsync(row, column, width, height);

							if (color.A > 0)
							{
								returnValue.Colors[row + startRow, column + startColumn] = color;
							}
						}
					}
				}
				else
				{
					throw new BadImageFormatException();
				}
			}

			return returnValue;
		}

		public async Task<bool> SaveAsync(ColorMatrix colorMatrix)
		{
			bool returnValue = false;

			byte[] data = await colorMatrix.CreateImageDataAsync(this.MaximumHeight, this.MaximumWidth);
			await data.CreateImageAsync(this.MaximumHeight, this.MaximumWidth, this.File);
			returnValue = true;

			return returnValue;
		}
	}
}
