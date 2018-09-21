using System;
using System.Threading.Tasks;
using ImageManager;
using Windows.Storage;

namespace CodeBuilder
{
	public interface IBuildService
	{
		event EventHandler<BuildEventArgs> BuildEvent;
		Task<bool> Build(StorageFolder folder, string projectName, ColorMatrix colorMatrix, ColorMatrix grainMatrix);
	}
}