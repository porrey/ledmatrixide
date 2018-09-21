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
using System.Collections.Generic;
using System.Windows.Input;
using LedMatrixIde.Helpers;
using LedMatrixIde.Services;
using Prism.Commands;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using Windows.ApplicationModel;
using Windows.UI.Xaml;

namespace LedMatrixIde.ViewModels
{
	public class SettingsViewModel : ViewModelBase
	{
		public SettingsViewModel()
		{
		}

		public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
		{
			base.OnNavigatedTo(e, viewModelState);
			this.VersionDescription = this.GetVersionDescription();
		}

		private ElementTheme _elementTheme = ThemeSelectorService.Theme;
		public ElementTheme ElementTheme
		{
			get
			{
				return _elementTheme;
			}
			set
			{
				this.SetProperty(ref _elementTheme, value);
			}
		}

		private string _versionDescription;
		public string VersionDescription
		{
			get
			{
				return _versionDescription;
			}
			set
			{
				this.SetProperty(ref _versionDescription, value);
			}
		}

		private ICommand _switchThemeCommand;
		public ICommand SwitchThemeCommand
		{
			get
			{
				if (_switchThemeCommand == null)
				{
					_switchThemeCommand = new DelegateCommand<object>(
						async (param) =>
						{
							this.ElementTheme = (ElementTheme)param;
							await ThemeSelectorService.SetThemeAsync((ElementTheme)param);
						});
				}

				return _switchThemeCommand;
			}
		}

		private string GetVersionDescription()
		{
			string appName = "AppDisplayName".GetLocalized();
			Package package = Package.Current;
			PackageId packageId = package.Id;
			PackageVersion version = packageId.Version;

			return $"{appName} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
		}

		private string _buildPath = String.Empty;
		public string BuildPath
		{
			get
			{
				return _buildPath;
			}
			set
			{
				this.SetProperty(ref _buildPath, value);
			}
		}
	}
}
