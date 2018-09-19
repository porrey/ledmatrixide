using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;

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
				BitmapDecoder dec = await BitmapDecoder.CreateAsync(imageStream.AsRandomAccessStream());
				PixelDataProvider data = await dec.GetPixelDataAsync();
				byte[] bytes = data.DetachPixelData();

				uint width = dec.OrientedPixelWidth;
				uint height = dec.OrientedPixelHeight;

				if (width >= this.MaximumWidth && height >= this.MaximumHeight)
				{
					WriteableBitmap bitmap = await imageStream.ResizeImageAsync(this.MaximumHeight, this.MaximumWidth);
					bytes = await bitmap.ToArrayAsync();

					height = this.MaximumHeight;
					width = this.MaximumWidth;
				}

				if (width <= this.MaximumWidth && height <= this.MaximumHeight)
				{
					int startColumn = (int)((this.MaximumWidth - dec.OrientedPixelWidth) / 2.0);
					int startRow = (int)((this.MaximumHeight - dec.OrientedPixelHeight) / 2.0);

					for (int row = 0; row < height; row++)
					{
						for (int column = 0; column < width; column++)
						{
							Color color = await this.GetPixelAsync(bytes, row, column, width, height);

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

			byte[] data = await this.CreateImageDataAsync(this.MaximumHeight, this.MaximumWidth, colorMatrix);
			await this.CreateImageAsync(this.MaximumHeight, this.MaximumWidth, data, this.File);
			returnValue = true;

			return returnValue;
		}

		protected async Task CreateImageAsync(uint height, uint width, byte[] data, StorageFile storageFile)
		{
			using (IRandomAccessStream stream = await storageFile.OpenAsync(FileAccessMode.ReadWrite))
			{
				BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
				encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, width, height, 96, 96, data);
				await encoder.FlushAsync();
			}
		}

		protected Task<Color> GetPixelAsync(byte[] pixels, int row, int column, uint width, uint height)
		{
			int index = (row * (int)width + column) * 4;

			byte b = pixels[index + 0];
			byte g = pixels[index + 1];
			byte r = pixels[index + 2];
			byte a = pixels[index + 3];

			return Task.FromResult(Color.FromArgb(a, r, g, b));
		}

		protected Task<byte[]> CreateImageDataAsync(uint height, uint width, ColorMatrix colorMatrix)
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
