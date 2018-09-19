using System;
using System.Threading.Tasks;
using ImageConverter;
using Windows.Storage;

namespace CodeBuilder
{
	public interface IBuilder
	{
		event EventHandler<BuildEventArgs> BuildEvent;
		Task<bool> Build(StorageFolder folder, string projectName, ColorMatrix colorMatrix, ColorMatrix grainMatrix);
	}
}