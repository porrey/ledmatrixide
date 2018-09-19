using LedMatrixIde.ViewModels;
using Windows.UI.Xaml.Controls;

namespace LedMatrixIde.Views
{
	public sealed partial class SettingsPage : Page
    {
        private SettingsViewModel ViewModel => DataContext as SettingsViewModel;

        public SettingsPage()
        {
			this.InitializeComponent();
        }
    }
}
