using System;
using System.Threading.Tasks;
using Project;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace ImageManager
{
	public static class ImageMetadataDecorator
	{
		public static async Task SaveProjectMetaData(this IMatrixProject project, StorageFile file)
		{
			ImageProperties properties = await file.Properties.GetImagePropertiesAsync();

			ResourceLoader resLoader = new ResourceLoader();

			properties.DateTaken = DateTimeOffset.Now;
			properties.CameraManufacturer = resLoader.GetString("AppDisplayName");
			properties.CameraModel = project.ColorMatrix.BackgroundColor.ToHexString();
			properties.Title = project.Name;

			await properties.SavePropertiesAsync();
		}

		public static async Task RestoreImageMetaData(this IMatrixProject project, StorageFile file)
		{
			ImageProperties properties = await file.Properties.GetImagePropertiesAsync();

			if (properties.CameraModel.IsHexColor())
			{
				project.ColorMatrix.BackgroundColor = properties.CameraModel.ToColor();
			}

			if (!String.IsNullOrEmpty(properties.Title))
			{
				project.Name = properties.Title;
			}
		}
	}
}
