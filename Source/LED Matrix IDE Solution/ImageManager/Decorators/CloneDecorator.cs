using System.Threading.Tasks;

namespace ImageManager
{
	public static class CloneDecorator
	{
		public static Task<ColorMatrix> CloneAsync(this ColorMatrix sourceColorMatrix)
		{
			ColorMatrix returnValue = new ColorMatrix(sourceColorMatrix.Height, sourceColorMatrix.Width);

			for (uint row = 0; row < sourceColorMatrix.Height; row++)
			{
				for (uint column = 0; column < sourceColorMatrix.Width; column++)
				{
					returnValue.ColorItems[column, row] = sourceColorMatrix.ColorItems[column, row];
				}
			}

			return Task.FromResult(returnValue);
		}
	}
}
