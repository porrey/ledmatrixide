using System.Text;
using System.Threading.Tasks;
using ImageManager;

namespace CodeBuilder.Decorators
{
	public static class CreateHeaderFileDecorator
	{
		public static Task<string> CreateHeaderFile(this IBuildProject project, IBuildService buildService)
		{
			StringBuilder returnValue = new StringBuilder();

			// ***
			// *** Generate the image definition.
			// ***
			buildService.FireBuildEvent(BuildEventArgs.BuildEventType.Information, "Generating image code.");
			string imageCode = project.CreateImageCode();

			// ***
			// *** Generate the mask definition.
			// ***
			buildService.FireBuildEvent(BuildEventArgs.BuildEventType.Information, "Generating mask code.");
			string maskCode = project.CreateMaskCode();

			// ***
			// *** Generate the sand grain definition.
			// ***
			buildService.FireBuildEvent(BuildEventArgs.BuildEventType.Information, "Generating grain code.");

			(bool result, string grainCode) = project.CreateGrainCode();

			if (!result)
			{
				buildService.FireBuildEvent(BuildEventArgs.BuildEventType.Warning, "No sand grains have been defined.");
			}

			buildService.FireBuildEvent(BuildEventArgs.BuildEventType.Information, "Generating header file code.");

			// ***
			// *** Generate the header file (.h)
			// ***
			returnValue.AppendLine(grainCode);
			returnValue.AppendLine();
			returnValue.AppendLine(imageCode);
			returnValue.AppendLine();
			returnValue.AppendLine(maskCode);

			return Task.FromResult(returnValue.ToString());
		}
	}
}
