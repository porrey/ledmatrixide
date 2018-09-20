using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace ImageConverter
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
