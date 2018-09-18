using System;
using System.Collections.Generic;
using System.Windows.Input;
using LedMatrixIde.Helpers;
using LedMatrixIde.Services;
using Prism.Commands;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Xaml;

namespace LedMatrixIde.ViewModels
{
	public class SettingsViewModel : ViewModelBase
	{
		public const string BuildPathKey = "BuildPathKey";

		public SettingsViewModel()
		{
			this.BrowseBuildPathCommand = new DelegateCommand(this.OnBrowseBuildPathCommand, this.OnEnableBrowseBuildPathCommand);
		}

		public override async void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
		{
			base.OnNavigatedTo(e, viewModelState);

			// ***
			// *** Get the build output folder.
			// ***
			string defaultPath = $@"{KnownFolders.DocumentsLibrary.Name}\LED Matrix Output";
			this.BuildPath = await ApplicationData.Current.LocalSettings.ReadAsync<string>(SettingsViewModel.BuildPathKey, defaultPath);

			this.VersionDescription = this.GetVersionDescription();
		}

		public override async void OnNavigatingFrom(NavigatingFromEventArgs e, Dictionary<string, object> viewModelState, bool suspending)
		{
			// ***
			// *** Save the build output folder.
			// ***
			await ApplicationData.Current.LocalSettings.SaveAsync<string>(SettingsViewModel.BuildPathKey, this.BuildPath);

			base.OnNavigatingFrom(e, viewModelState, suspending);
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

		public DelegateCommand BrowseBuildPathCommand { get; set; }

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

		protected bool OnEnableBrowseBuildPathCommand()
		{
			return true;
		}

		protected void OnBrowseBuildPathCommand()
		{

		}
	}
}
