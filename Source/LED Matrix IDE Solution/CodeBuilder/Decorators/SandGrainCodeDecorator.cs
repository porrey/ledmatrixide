// Copyright © 2018 Daniel Porrey. All Rights Reserved.
//
// This file is part of the LED Matrix IDE Solution project.
// 
// The LED Matrix IDE Solution is free software: you can redistribute it
// and/or modify it under the terms of the GNU General Public License as
// published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
// 
// The LED Matrix IDE Solution is distributed in the hope that it will
// be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with the LED Matrix IDE Solution. If not, 
// see http://www.gnu.org/licenses/.
//
using System;
using System.Text;
using ImageManager;
using Matrix;
using Project;
using Windows.UI;

namespace CodeBuilder.Decorators
{
	public static class SandGrainCodeDecorator
	{
		public static (bool, string) CreateGrainCode(this IMatrixProject project)
		{
			(bool result, string text) = (false, String.Empty);

			StringBuilder code = new StringBuilder();
			uint grainCount = 0;

			if (!project.UseRandomSand)
			{
				 grainCount = project.ColorMatrix.GetPixelCount(ColorItem.ColorItemType.Sand);
			}
			else
			{
				grainCount = project.RandomSandCount;
			}

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

			if (!project.UseRandomSand)
			{
				uint index = 0;

				for (uint row = 0; row < project.ColorMatrix.Height; row++)
				{
					for (uint column = 0; column < project.ColorMatrix.Width; column++)
					{
						ColorItem colorItem = project.ColorMatrix.ColorItems[row, column];

						if (colorItem.ItemType == ColorItem.ColorItemType.Sand)
						{
							Color color = colorItem;
							code.Append($"\t{column.ToString().PadLeft(2, ' ')}, {row.ToString().PadLeft(2, ' ')}, {color.ToHexInt()}");
							index++;

							if (index < grainCount)
							{
								code.AppendLine(",");
							}
						}
					}
				}

				code.AppendLine();
			}

			code.AppendLine("};");
			code.AppendLine();
			code.AppendLine("// ***");
			code.AppendLine("// *** Determines if the initial sand");
			code.AppendLine("// *** position(s) are randomized.");
			code.AppendLine("// ***");

			if (project.UseRandomSand)
			{
				code.Append("#define USE_RANDOM_SAND 1");
			}
			else
			{
				code.Append("#define USE_RANDOM_SAND 0");
			}

			result = project.UseRandomSand ? true : (grainCount > 0);

			return (result, code.ToString());
		}
	}
}
