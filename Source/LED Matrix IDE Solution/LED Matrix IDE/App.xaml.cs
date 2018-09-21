using System;
using System.Globalization;
using System.Threading.Tasks;
using CodeBuilder;
using LedMatrixIde.Interfaces;
using LedMatrixIde.Services;
using LedMatrixIde.Views;
using Microsoft.Practices.Unity;
using Prism.Mvvm;
using Prism.Unity.Windows;
using Prism.Windows.AppModel;
using Prism.Windows.Navigation;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Resources;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LedMatrixIde
{
	[Windows.UI.Xaml.Data.Bindable]
    public sealed partial class App : PrismUnityApplication
    {
        public App()
        {
			this.InitializeComponent();
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

			this.Container.RegisterInstance<IResourceLoader>(new ResourceLoaderAdapter(new ResourceLoader()));
			this.Container.RegisterType<IUndoService, UndoService>();
			this.Container.RegisterType<IBuildService, BuildService>();
		}

        protected override async Task OnLaunchApplicationAsync(LaunchActivatedEventArgs args)
        {
            await this.LaunchApplicationAsync(PageTokens.ImageEditorPage, null);
        }

        private async Task LaunchApplicationAsync(string page, object launchParam)
        {
            ThemeSelectorService.SetRequestedTheme();

			this.SessionStateService.RegisterKnownType(typeof(Color));


			this.NavigationService.Navigate(page, launchParam);
            Window.Current.Activate();
            await Task.CompletedTask;
        }

        protected override async Task OnActivateApplicationAsync(IActivatedEventArgs args)
        {
            await Task.CompletedTask;
        }

        protected override async Task OnInitializeAsync(IActivatedEventArgs args)
        {
            await ThemeSelectorService.InitializeAsync().ConfigureAwait(false);

            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
            {
				string viewModelTypeName = String.Format(CultureInfo.InvariantCulture, "LedMatrixIde.ViewModels.{0}ViewModel, LedMatrixIde", viewType.Name.Substring(0, viewType.Name.Length - 4));
                return Type.GetType(viewModelTypeName);
            });

            await base.OnInitializeAsync(args);
        }

        public void SetNavigationFrame(Frame frame)
        {
			ISessionStateService sessionStateService = this.Container.Resolve<ISessionStateService>();
			this.CreateNavigationService(new FrameFacadeAdapter(frame), sessionStateService);
        }

        protected override UIElement CreateShell(Frame rootFrame)
        {
			ShellPage shell = this.Container.Resolve<ShellPage>();
            shell.SetRootFrame(rootFrame);
            return shell;
        }
    }
}
