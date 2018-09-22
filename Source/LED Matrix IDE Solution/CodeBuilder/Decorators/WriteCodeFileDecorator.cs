using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources.Core;
using Windows.Storage;

namespace CodeBuilder.Decorators
{
	public static class WriteCodeFileDecorator
	{
		public static async Task WriteCodeFile(this IBuildProject project, StorageFolder folder,  IBuildService buildService)
		{
			buildService.FireBuildEvent(BuildEventArgs.BuildEventType.Information, "Reading code template.");
			ResourceContext resourceContext = ResourceContext.GetForViewIndependentUse();
			ResourceMap resourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("CodeBuilder/Code");
			ResourceCandidate resourceValue = resourceMap.GetValue("cpp", resourceContext);

			string template = resourceValue.ValueAsString;
			string contents = String.Format(template, project.Name, project.ColorMatrix.BackgroundColor.R, project.ColorMatrix.BackgroundColor.G, project.ColorMatrix.BackgroundColor.B);

			buildService.FireBuildEvent(BuildEventArgs.BuildEventType.Information, $"Writing C++ code file '{project.CppFileName()}'.");
			StorageFile file = await folder.CreateFileAsync(project.CppFileName(), CreationCollisionOption.ReplaceExisting);
			await FileIO.WriteTextAsync(file, contents, Windows.Storage.Streams.UnicodeEncoding.Utf8);
		}
	}
}
