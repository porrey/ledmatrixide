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
using Windows.ApplicationModel.Resources.Core;
using Windows.Storage;

namespace CodeBuilder.Decorators
{
	public static class WriteCodeFileDecorator
	{
		public static async Task WriteCodeFile(this IMatrixProject project, StorageFolder folder,  IBuildService buildService)
		{
			buildService.FireBuildEvent(BuildEventArgs.BuildEventType.Information, "Reading code template.");
			ResourceContext resourceContext = ResourceContext.GetForViewIndependentUse();
			ResourceMap resourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("CodeBuilder/Code");
			ResourceCandidate resourceValue = resourceMap.GetValue("cpp", resourceContext);

			string template = resourceValue.ValueAsString;
			string contents = String.Format(template, project.Name, project.ColorMatrix.BackgroundColor.R, project.ColorMatrix.BackgroundColor.G, project.ColorMatrix.BackgroundColor.B);

			buildService.FireBuildEvent(BuildEventArgs.BuildEventType.Information, $"Writing C++ code file '{project.CppFileName()}'.");
			StorageFile file = await folder.CreateFileAsync(project.CppFileName(), CreationCollisionOption.ReplaceExisting);
			await FileIO.WriteTextAsync(file, contents, Windows.Storage.Streams.UnicodeEncoding.Utf8);
		}
	}
}
