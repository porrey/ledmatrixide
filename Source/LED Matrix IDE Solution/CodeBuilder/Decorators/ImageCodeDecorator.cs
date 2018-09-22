using System.Text;
using ImageManager;
using Windows.UI;

namespace CodeBuilder.Decorators
{
	public static class ImageCodeDecorator
	{
		public static string CreateImageCode(this IBuildProject project)
		{
			StringBuilder returnValue = new StringBuilder();

			returnValue.AppendLine($"#define IMAGE_WIDTH  {project.ColorMatrix.Width}");
			returnValue.AppendLine($"#define IMAGE_HEIGHT {project.ColorMatrix.Height}");
			returnValue.AppendLine();
			returnValue.AppendLine("const uint32_t image_color[IMAGE_HEIGHT][IMAGE_WIDTH] =");
			returnValue.AppendLine("{");

			int i = 0;

			for (uint row = 0; row < project.ColorMatrix.Height; row++)
			{
				for (uint column = 0; column < project.ColorMatrix.Width; column++)
				{
					ColorItem colorItem = project.ColorMatrix.ColorItems[row, column];
					Color color = colorItem;

					if (i == 0)
					{
						returnValue.Append("\t");
					}

					if (colorItem.ItemType == ColorItem.ColorItemType.Background ||
						colorItem.ItemType == ColorItem.ColorItemType.Sand)
					{
						returnValue.Append("0x00000000");
					}
					else
					{
						returnValue.Append($"{color.ColorToHex()}");
					}

					if (i < ((project.ColorMatrix.Height * project.ColorMatrix.Width) - 1))
					{
						returnValue.Append(", ");
					}

					if ((i + 1) % project.PixelColumns == 0)
					{
						returnValue.AppendLine();
						returnValue.Append("\t");
					}

					i++;
				}
			}

			returnValue.AppendLine();
			returnValue.Append("};");

			return returnValue.ToString();
		}
	}
}
