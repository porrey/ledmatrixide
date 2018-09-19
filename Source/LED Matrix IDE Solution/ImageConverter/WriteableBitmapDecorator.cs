using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace ImageConverter
{
	public static class WriteableBitmapDecorator
	{
		public static async Task<WriteableBitmap> ToBitmapAsync(this byte[] data, uint width, uint height)
		{
			WriteableBitmap returnValue = null;

			returnValue = new WriteableBitmap((int)width, (int)height);

			using (Stream stream = returnValue.PixelBuffer.AsStream())
			{
				await stream.WriteAsync(data, 0, data.Length);
			}

			return returnValue;
		}

		public static async Task<byte[]> ToArrayAsync(this WriteableBitmap bitmap)
		{
			// ***
			// *** WriteableBitmap uses BGRA format which is 4 bytes per pixel.
			// ***
			byte[] returnValue = new byte[bitmap.PixelWidth * bitmap.PixelHeight * 4];

			using (Stream stream = bitmap.PixelBuffer.AsStream())
			{
				await stream.ReadAsync(returnValue, 0, returnValue.Length);
			}

			return returnValue;
		}

		public static async Task<WriteableBitmap> ResizeImageAsync(this Stream imageStream, uint targetHeight, uint targetWidth)
		{
			WriteableBitmap returnValue = null;

			using (IRandomAccessStream randomAccessStream = imageStream.AsRandomAccessStream())
			{
				BitmapDecoder decoder = await BitmapDecoder.CreateAsync(randomAccessStream);

				using (InMemoryRandomAccessStream resizedStream = new InMemoryRandomAccessStream())
				{
					BitmapEncoder encoder = await BitmapEncoder.CreateForTranscodingAsync(resizedStream, decoder);

					encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Linear;
					encoder.BitmapTransform.ScaledHeight = targetHeight;
					encoder.BitmapTransform.ScaledWidth = targetWidth;

					await encoder.FlushAsync();
					resizedStream.Seek(0);
					byte[] outBuffer = new byte[resizedStream.Size];

					returnValue = await outBuffer.ToBitmapAsync(targetWidth, targetHeight);
				}
			}

			return returnValue;
		}
	}
}
