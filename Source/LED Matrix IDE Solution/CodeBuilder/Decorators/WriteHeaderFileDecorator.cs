using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace CodeBuilder.Decorators
{
	public static class WriteHeaderFileDecorator
	{
		public static async Task WriteHeaderFile(this IBuildProject project, StorageFolder folder,  string headerCode, IBuildService buildService)
		{
			// ***
			// *** Write the header file (.h)
			// ***
			buildService.FireBuildEvent(BuildEventArgs.BuildEventType.Information, $"Writing .h code file '{project.HeaderFileName()}'.");
			StorageFile hFile = await folder.CreateFileAsync(project.HeaderFileName(), CreationCollisionOption.ReplaceExisting);
			await FileIO.WriteTextAsync(hFile, headerCode, Windows.Storage.Streams.UnicodeEncoding.Utf8);
		}
	}
}
