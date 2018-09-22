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
using CodeBuilder.Decorators;
using ImageManager;
using Windows.Storage;

namespace CodeBuilder
{
	public class BuildService : IBuildService
	{
		public event EventHandler<BuildEventArgs> BuildEvent = null;

		public async Task<bool> Build(IBuildProject project, StorageFolder folder)
		{
			bool returnValue = true;

			this.OnBuildEvent(BuildEventArgs.BuildEventType.Information, $"Build started.");

			// ***
			// *** Get the .cpp code, format it and write the file.
			// ***
			await project.WriteCodeFile(folder, this);

			// ***
			// *** Create make file text file.
			// ***
			string headerCode = await project.CreateHeaderFile(this);
			await project.WriteHeaderFile(folder, headerCode, this);

			// ***
			// *** Create make file text file.
			// ***
			await project.WriteMakeFile(folder, this);

			// ***
			// *** Create make file text file.
			// ***
			await project.WriteInstructionsFile(folder, this);

			this.OnBuildEvent(BuildEventArgs.BuildEventType.Information, $"Completed.");

			return returnValue;
		}

		public void FireBuildEvent(BuildEventArgs.BuildEventType eventType, string message)
		{
			this.OnBuildEvent(eventType, message);
		}

		protected void OnBuildEvent(BuildEventArgs.BuildEventType eventType, string message)
		{
			this.BuildEvent?.Invoke(this, new BuildEventArgs(eventType, message));
		}
	}
}
