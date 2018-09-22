using System;
using System.Text;
using ImageManager;
using Windows.UI;

namespace CodeBuilder.Decorators
{
	public static class SandGrainCodeDecorator
	{
		public static (bool, string) CreateGrainCode(this IBuildProject project)
		{
			(bool result, string text) = (false, String.Empty);

			StringBuilder code = new StringBuilder();

			uint grainCount = project.ColorMatrix.GetPixelCount(ColorItem.ColorItemType.Sand);

			code.AppendLine("// ***");
			code.AppendLine("// *** Define the number of grains of sand on matrix.");
			code.AppendLine("// ***");
			code.AppendLine($"#define NUM_GRAINS {grainCount}");
			code.AppendLine();
			code.AppendLine("// ***");
			code.AppendLine("// *** Defines the initial position and color");
			code.AppendLine("// *** of each grain (x, y, color).");
			code.AppendLine("// ***");
			code.AppendLine("const uint32_t grains[NUM_GRAINS][3] =");
			code.AppendLine("{");

			uint index = 0;

			for (uint row = 0; row < project.ColorMatrix.Height; row++)
			{
				for (uint column = 0; column < project.ColorMatrix.Width; column++)
				{
					ColorItem colorItem = project.ColorMatrix.ColorItems[row, column];

					if (colorItem.ItemType == ColorItem.ColorItemType.Sand)
					{
						Color color = colorItem;
						code.Append($"\t{column.ToString().PadLeft(2, ' ')}, {row.ToString().PadLeft(2, ' ')}, {color.ColorToHex()}");
						index++;

						if (index < grainCount)
						{
							code.AppendLine(",");
						}
					}
				}
			}

			result = (grainCount > 0);

			code.AppendLine();
			code.Append("};");

			return (result, code.ToString());
		}
	}
}
