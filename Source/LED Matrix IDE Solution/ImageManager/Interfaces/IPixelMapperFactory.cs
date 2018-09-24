using System.Threading.Tasks;
using Windows.Graphics.Imaging;

namespace ImageManager
{
	public interface IPixelMapperFactory
	{
		Task<IPixelMapper> GetMapper(BitmapPixelFormat format);
	}
}