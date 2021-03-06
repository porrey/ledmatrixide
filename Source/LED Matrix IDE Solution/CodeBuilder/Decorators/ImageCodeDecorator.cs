﻿// Copyright © 2018 Daniel Porrey. All Rights Reserved.
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
using System.Text;
using ImageManager;
using Matrix;
using Project;
using Windows.UI;

namespace CodeBuilder.Decorators
{
	public static class ImageCodeDecorator
	{
		public static string CreateImageCode(this IMatrixProject project)
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
						returnValue.Append($"{color.ToHexInt()}");
					}

					if (i < ((project.ColorMatrix.Height * project.ColorMatrix.Width) - 1))
					{
						returnValue.Append(", ");
					}

					if ((i + 1) % project.PixelOutputColumns == 0)
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
