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
using System;
using System.Text;
using System.Threading.Tasks;
using Project;
using Windows.Storage;

namespace CodeBuilder.Decorators
{
	public static class WriteMakeFileDecorator
	{
		public static async Task WriteMakeFile(this IMatrixProject project, StorageFolder folder, IBuildService buildService)
		{
			StringBuilder contents = new StringBuilder();
			contents.AppendLine($"{project.Name}: {project.CppFileName()} {project.HeaderFileName()} $(LIBS)");
			contents.AppendLine($"\t$(CXX) $(CXXFLAGS) $< $(LDFLAGS) $(LIBS) -o $@");
			contents.AppendLine($"\tstrip $@");

			buildService.FireBuildEvent(BuildEventArgs.BuildEventType.Information, $"Writing make file '{project.MakeFileName()}'.");
			StorageFile file = await folder.CreateFileAsync(project.MakeFileName(), CreationCollisionOption.ReplaceExisting);
			await FileIO.WriteTextAsync(file, contents.ToString(), Windows.Storage.Streams.UnicodeEncoding.Utf8);
		}
	}
}
