using ImageManager;
using Windows.UI;

namespace CodeBuilder
{
	public interface IBuildProject
	{
		string Name { get; }
		ColorMatrix ColorMatrix { get; }
		uint PixelColumns { get; }
		uint MaskColumns { get; }
	}
}
