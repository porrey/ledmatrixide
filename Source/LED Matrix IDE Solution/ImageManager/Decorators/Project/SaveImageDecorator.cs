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
using System.Threading.Tasks;
using Project;
using Windows.Storage;

namespace ImageManager
{
	public static class SaveImageDecorator
	{
		public static async Task<bool> SaveAsync(this IMatrixProject project, StorageFile file)
		{
			bool returnValue = false;

			// ***
			// *** The data is always mapped to BGRA8 pixel format.
			// ***
			byte[] data = await project.ColorMatrix.CreateImageDataAsync();
			await data.CreateImageAsync(project.ColorMatrix.Height, project.ColorMatrix.Width, file);
			await project.SaveProjectMetaData(file);
			returnValue = true;

			return returnValue;
		}
	}
}
