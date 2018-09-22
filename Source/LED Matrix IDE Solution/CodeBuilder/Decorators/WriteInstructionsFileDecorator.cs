using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources.Core;
using Windows.Storage;

namespace CodeBuilder.Decorators
{
	public static class WriteInstructionsFileDecorator
	{
		public static async Task WriteInstructionsFile(this IBuildProject project, StorageFolder folder, IBuildService buildService)
		{
			buildService.FireBuildEvent(BuildEventArgs.BuildEventType.Information, "Reading instructions template.");
			ResourceContext resourceContext = ResourceContext.GetForViewIndependentUse();
			ResourceMap resourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("CodeBuilder/Code");
			ResourceCandidate resourceValue = resourceMap.GetValue("instructions", resourceContext);

			string template = resourceValue.ValueAsString;
			string contents = String.Format(template, project.CppFileName(),
													  project.HeaderFileName(),
													  project.MakeFileName(),
													  project.Name);

			buildService.FireBuildEvent(BuildEventArgs.BuildEventType.Information, $"Writing instructions file '{project.InstructionsFileName()}'.");
			StorageFile file = await folder.CreateFileAsync(project.InstructionsFileName(), CreationCollisionOption.ReplaceExisting);
			await FileIO.WriteTextAsync(file, contents, Windows.Storage.Streams.UnicodeEncoding.Utf8);
		}
	}
}
