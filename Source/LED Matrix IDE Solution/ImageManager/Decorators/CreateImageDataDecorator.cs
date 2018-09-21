using System.Threading.Tasks;

namespace ImageManager
{
	public static class CreateImageDataDecorator
	{
		/// <summary>
		/// Convert the ColorMatrix to a BGRA array.
		/// </summary>
		/// <param name="height"></param>
		/// <param name="width"></param>
		/// <returns></returns>
		public static Task<byte[]> CreateImageDataAsync(this ColorMatrix sourceColorMatrix, uint height, uint width)
		{
			byte[] returnValue = new byte[height * width * 4];
			int index = 0;

			for (int row = 0; row < height; row++)
			{
				for (int column = 0; column < width; column++)
				{
					returnValue[index + 0] = sourceColorMatrix.ColorItems[row, column].B;
					returnValue[index + 1] = sourceColorMatrix.ColorItems[row, column].G;
					returnValue[index + 2] = sourceColorMatrix.ColorItems[row, column].R;
					returnValue[index + 3] = sourceColorMatrix.ColorItems[row, column].A;
					index += 4;
				}
			}

			return Task.FromResult(returnValue);
		}
	}
}
