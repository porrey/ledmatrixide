using System.Threading.Tasks;
using Windows.UI;

namespace ImageManager
{
	/// <summary>
	/// Provides pixel mapping where the decoded bytes array is 8 bit per color and the colors
	/// are in the order blue, green, red and then alpha.
	/// </summary>
	public class Bgra8Pixelmapper : IPixelMapper
	{
		/// <summary>
		/// Gets the pixel for the specified row and column (matrix format) for a given
		/// matrix of width and height.
		/// </summary>
		/// <param name="decodedBytes">A single dimension array containing the
		/// color data from the image.</param>
		/// <param name="row">The pixel row of the matrix requested.</param>
		/// <param name="column">The pixel column of the matrix requested.</param>
		/// <param name="width">The width of the matrix in pixels.</param>
		/// <param name="height">The height of the matrix in pixels.</param>
		/// <returns></returns>
		public Task<Color> GetPixelAsync(byte[] decodedBytes, uint row, uint column, uint width, uint height)
		{
			uint index = (row * (uint)width + column) * 4;

			byte b = decodedBytes[index + 0];
			byte g = decodedBytes[index + 1];
			byte r = decodedBytes[index + 2];
			byte a = decodedBytes[index + 3];

			return Task.FromResult(Color.FromArgb(a, r, g, b));
		}
	}
}
