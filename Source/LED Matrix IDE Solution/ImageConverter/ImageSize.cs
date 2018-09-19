using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;

namespace ImageConverter
{
	public class ImageSize
	{
		public async Task<StorageFile> RescaleImage(StorageFile sourceFile, StorageFile resizedImageFile, uint width, uint height, BitmapInterpolationMode mode)
		{
			IRandomAccessStreamWithContentType imageStream = await sourceFile.OpenReadAsync();
			BitmapDecoder decoder = await BitmapDecoder.CreateAsync(imageStream);

			using (IRandomAccessStream resizedStream = await resizedImageFile.OpenAsync(FileAccessMode.ReadWrite))
			{
				BitmapEncoder encoder = await BitmapEncoder.CreateForTranscodingAsync(resizedStream, decoder);

				encoder.BitmapTransform.InterpolationMode = mode;
				encoder.BitmapTransform.ScaledWidth = width;
				encoder.BitmapTransform.ScaledHeight = height;

				await encoder.FlushAsync();
			}

			return resizedImageFile;
		}

		public async Task<byte[]> ResizeImage(byte[] imageData, uint width, uint height, int quality)
		{
			byte[] returnValue = null;

			using (MemoryStream memStream = new MemoryStream(imageData))
			{
				using (IRandomAccessStream imageStream = memStream.AsRandomAccessStream())
				{
					BitmapDecoder decoder = await BitmapDecoder.CreateAsync(imageStream);

					if (decoder.PixelHeight > height || decoder.PixelWidth > width)
					{
						using (imageStream)
						{
							using (InMemoryRandomAccessStream resizedStream = new InMemoryRandomAccessStream())
							{
								BitmapEncoder encoder = await BitmapEncoder.CreateForTranscodingAsync(resizedStream, decoder);
								double widthRatio = (double)width / decoder.PixelWidth;
								double heightRatio = (double)height / decoder.PixelHeight;

								double scaleRatio = Math.Min(widthRatio, heightRatio);

								if (width == 0)
								{
									scaleRatio = heightRatio;
								}

								if (height == 0)
								{
									scaleRatio = widthRatio;
								}

								uint aspectHeight = (uint)Math.Floor(decoder.PixelHeight * scaleRatio);
								uint aspectWidth = (uint)Math.Floor(decoder.PixelWidth * scaleRatio);

								encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Linear;

								encoder.BitmapTransform.ScaledHeight = aspectHeight;
								encoder.BitmapTransform.ScaledWidth = aspectWidth;

								await encoder.FlushAsync();
								resizedStream.Seek(0);
								byte[] outBuffer = new byte[resizedStream.Size];

								//uint x = await resizedStream.WriteAsync(outBuffer.AsBuffer());
								returnValue = outBuffer;
							}
						}
					}
				}

				return returnValue;
			}

			return imageData;
		}
	}
}
