using System.Threading.Tasks;

namespace ImageManager
{
	public static class RotateClockwiseDecorator
	{
		public static async Task RotateClockwiseAsync(this ColorMatrix sourceColorMatrix)
		{
			ColorMatrix me = await sourceColorMatrix.CloneAsync();

			for (uint row = 0; row < sourceColorMatrix.Height; row++)
			{
				for (uint column = 0; column < sourceColorMatrix.Width; column++)
				{
					await sourceColorMatrix.SetItem(row, column, me.ColorItems[(sourceColorMatrix.Height - 1) - column, row]);
				}
			}
		}
	}
}
