using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
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
			properties.Title = project.Name;
			properties.CameraModel = JsonConvert.SerializeObject(project);

			await properties.SavePropertiesAsync();
		}

		public static async Task RestoreImageMetaData(this IMatrixProject project, StorageFile file)
		{
			ImageProperties properties = await file.Properties.GetImagePropertiesAsync();

			if (properties.CameraModel.IsHexColor())
			{
				project.ColorMatrix.BackgroundColor = properties.CameraModel.ToColor();

				if (!String.IsNullOrEmpty(properties.Title))
				{
					project.Name = properties.Title;
				}
			}
			else
			{
				if (properties.CameraModel.Substring(0, 1) == "{")
				{
					IMatrixProject projectProperties = JsonConvert.DeserializeObject<MatrixProject>(properties.CameraModel);
					project.AccelerometerScaling = projectProperties.AccelerometerScaling;
					project.Elasticity = projectProperties.Elasticity;
					project.MaskOutputColumns = projectProperties.MaskOutputColumns;
					project.Name = projectProperties.Name;
					project.PixelOutputColumns = projectProperties.PixelOutputColumns;
					project.RandomSandCount = projectProperties.RandomSandCount;
					project.SortParticles = projectProperties.SortParticles;
					project.UseRandomSand = projectProperties.UseRandomSand;
				}
			}
		}
	}
}
