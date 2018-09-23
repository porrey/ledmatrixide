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
using System.Threading.Tasks;
using Project;
using Windows.Storage;

namespace CodeBuilder.Decorators
{
	public static class WriteHeaderFileDecorator
	{
		public static async Task WriteHeaderFile(this IMatrixProject project, StorageFolder folder,  string headerCode, IBuildService buildService)
		{
			// ***
			// *** Write the header file (.h)
			// ***
			buildService.FireBuildEvent(BuildEventArgs.BuildEventType.Information, $"Writing .h code file '{project.HeaderFileName()}'.");
			StorageFile hFile = await folder.CreateFileAsync(project.HeaderFileName(), CreationCollisionOption.ReplaceExisting);
			await FileIO.WriteTextAsync(hFile, headerCode, Windows.Storage.Streams.UnicodeEncoding.Utf8);
		}
	}
}
