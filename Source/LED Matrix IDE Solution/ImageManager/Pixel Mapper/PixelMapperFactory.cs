using System;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;

namespace ImageManager
{
	public class PixelMapperFactory : IPixelMapperFactory
	{
		public Task<IPixelMapper> GetMapper(BitmapPixelFormat format)
		{
			IPixelMapper returnValue = null;

			if (format == BitmapPixelFormat.Bgra8)
			{
				returnValue = new Bgra8Pixelmapper();
			}
			else
			{
				throw new NotSupportedException($"The pixel format {format} is not supported.");
			}

			return Task.FromResult(returnValue);
		}
	}
}
