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
using System.Text;
using System.Threading.Tasks;
using Project;

namespace CodeBuilder.Decorators
{
	public static class CreateHeaderFileDecorator
	{
		public static Task<string> CreateHeaderFile(this IMatrixProject project, IBuildService buildService)
		{
			StringBuilder returnValue = new StringBuilder();

			// ***
			// *** Generate the image definition.
			// ***
			buildService.FireBuildEvent(BuildEventArgs.BuildEventType.Information, "Generating image code.");
			string imageCode = project.CreateImageCode();

			// ***
			// *** Generate the mask definition.
			// ***
			buildService.FireBuildEvent(BuildEventArgs.BuildEventType.Information, "Generating mask code.");
			string maskCode = project.CreateMaskCode();

			// ***
			// *** Generate the sand grain definition.
			// ***
			buildService.FireBuildEvent(BuildEventArgs.BuildEventType.Information, "Generating grain code.");

			(bool result, string grainCode) = project.CreateGrainCode();

			if (!result)
			{
				buildService.FireBuildEvent(BuildEventArgs.BuildEventType.Warning, "No sand grains have been defined.");
			}

			buildService.FireBuildEvent(BuildEventArgs.BuildEventType.Information, "Generating header file code.");

			// ***
			// *** Generate the header file (.h)
			// ***
			returnValue.AppendLine(grainCode);
			returnValue.AppendLine();
			returnValue.AppendLine(imageCode);
			returnValue.AppendLine();
			returnValue.AppendLine(maskCode);

			return Task.FromResult(returnValue.ToString());
		}
	}
}
