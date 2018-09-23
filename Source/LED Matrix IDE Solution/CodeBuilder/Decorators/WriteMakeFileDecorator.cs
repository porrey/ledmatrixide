using System;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace CodeBuilder.Decorators
{
	public static class WriteMakeFileDecorator
	{
		public static async Task WriteMakeFile(this IBuildProject project, StorageFolder folder, IBuildService buildService)
		{
			StringBuilder contents = new StringBuilder();
			contents.AppendLine($"{project.Name}: {project.CppFileName()} {project.HeaderFileName()} $(LIBS)");
			contents.AppendLine($"\t$(CXX) $(CXXFLAGS) $< $(LDFLAGS) $(LIBS) -o $@");
			contents.AppendLine($"\tstrip $@");

			buildService.FireBuildEvent(BuildEventArgs.BuildEventType.Information, $"Writing make file '{project.MakeFileName()}'.");
			StorageFile file = await folder.CreateFileAsync(project.MakeFileName(), CreationCollisionOption.ReplaceExisting);
			await FileIO.WriteTextAsync(file, contents.ToString(), Windows.Storage.Streams.UnicodeEncoding.Utf8);
		}
	}
}
