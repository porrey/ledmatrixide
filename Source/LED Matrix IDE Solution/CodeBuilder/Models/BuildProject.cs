using ImageManager;
using Windows.UI;

namespace CodeBuilder.Models
{
	public class BuildProject : IBuildProject
	{
		public string Name { get; set; }
		public ColorMatrix ColorMatrix { get; set; }
		public uint PixelColumns { get; set; }
		public uint MaskColumns { get; set; }
	}
}
