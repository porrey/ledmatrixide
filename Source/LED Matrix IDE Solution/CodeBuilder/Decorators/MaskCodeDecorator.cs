using System.Text;
using ImageManager;

namespace CodeBuilder.Decorators
{
	public static class MaskCodeDecorator
	{
		public static string CreateMaskCode(this IBuildProject project)
		{
			StringBuilder returnValue = new StringBuilder();

			returnValue.AppendLine("const uint8_t image_mask[IMAGE_HEIGHT][IMAGE_WIDTH] =");
			returnValue.AppendLine("{");

			int i = 0;

			for (uint row = 0; row < project.ColorMatrix.Height; row++)
			{
				for (uint column = 0; column < project.ColorMatrix.Width; column++)
				{
					ColorItem colorItem = project.ColorMatrix.ColorItems[row, column];

					if (i == 0)
					{
						returnValue.Append("\t");
					}

					if (colorItem.ItemType == ColorItem.ColorItemType.Background ||
						colorItem.ItemType == ColorItem.ColorItemType.Sand)
					{
						returnValue.Append($"0");
					}
					else
					{
						returnValue.Append($"1");
					}

					if (i < ((project.ColorMatrix.Height * project.ColorMatrix.Width) - 1))
					{
						returnValue.Append(", ");
					}

					if ((i + 1) % project.MaskColumns == 0)
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
