using System.Threading.Tasks;

namespace ImageManager
{
	public static class CopyToDecorator
	{
		public static async Task CopyTo(this ColorMatrix sourceColorMatrix, ColorMatrix targetColorMatrix)
		{
			for (uint row = 0; row < sourceColorMatrix.Height; row++)
			{
				for (uint column = 0; column < sourceColorMatrix.Width; column++)
				{
					await targetColorMatrix.SetItem(row, column, sourceColorMatrix.ColorItems[row, column]);
				}
			}
		}
	}
}
