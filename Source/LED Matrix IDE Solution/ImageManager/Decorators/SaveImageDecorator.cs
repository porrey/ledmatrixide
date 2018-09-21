using System.Threading.Tasks;
using Windows.Storage;

namespace ImageManager
{
	public static class SaveImageDecorator
	{
		public static async Task<bool> SaveAsync(this ColorMatrix sourceColorMatrix, StorageFile file, uint maximumHeight, uint maximumWidth)
		{
			bool returnValue = false;

			byte[] data = await sourceColorMatrix.CreateImageDataAsync(maximumHeight, maximumWidth);
			await data.CreateImageAsync(maximumHeight, maximumWidth, file);
			returnValue = true;

			return returnValue;
		}
	}
}
