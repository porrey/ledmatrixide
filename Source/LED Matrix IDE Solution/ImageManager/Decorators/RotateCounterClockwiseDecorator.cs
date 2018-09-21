using System.Threading.Tasks;

namespace ImageManager
{
	public static class RotateCounterClockwiseDecorator
	{
		public static async Task RotateCounterClockwiseAsync(this ColorMatrix sourceColorMatrix)
		{
			ColorMatrix me = await sourceColorMatrix.CloneAsync();

			for (uint row = 0; row < sourceColorMatrix.Height; row++)
			{
				for (uint column = 0; column < sourceColorMatrix.Width; column++)
				{
					await sourceColorMatrix.SetItem(row, column, me.ColorItems[column, (sourceColorMatrix.Height - 1) - row]);
				}
			}
		}
	}
}
