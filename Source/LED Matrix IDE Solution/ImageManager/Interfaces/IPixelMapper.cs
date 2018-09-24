using System.Threading.Tasks;
using Windows.UI;

namespace ImageManager
{
	public interface IPixelMapper
	{
		Task<Color> GetPixelAsync(byte[] decodedBytes, uint row, uint column, uint width, uint height);
	}
}
