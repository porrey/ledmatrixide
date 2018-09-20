using System;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;

namespace ImageConverter
{
	public static class DecodedBytesDecorator
	{
		public static Task<Color> GetPixelAsync(this byte[] decodedBytes, int row, int column, uint width, uint height)
		{
			int index = (row * (int)width + column) * 4;

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
				BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
				encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, width, height, 96, 96, decodedBytes);
				await encoder.FlushAsync();
			}
		}
	}
}
