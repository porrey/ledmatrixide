using System.Threading.Tasks;

namespace ImageManager
{
	public static class CopyFromDecorator
	{
		public static async Task CopyFrom(this ColorMatrix targetColorMatrix, ColorMatrix sourceColorMatrix)
		{
			for (uint row = 0; row < targetColorMatrix.Height; row++)
			{
				for (uint column = 0; column < targetColorMatrix.Width; column++)
				{
					await targetColorMatrix.SetItem(row, column, sourceColorMatrix.ColorItems[row, column]);
				}
			}
		}
	}
}
