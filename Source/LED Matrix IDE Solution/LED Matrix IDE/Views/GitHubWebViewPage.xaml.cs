using LedMatrixIde.Services;
using LedMatrixIde.ViewModels;
using Windows.UI.Xaml.Controls;

namespace LedMatrixIde.Views
{
	public sealed partial class GitHubWebViewPage : Page
    {
        private GitHubWebViewViewModel ViewModel => this.DataContext as GitHubWebViewViewModel;

        public GitHubWebViewPage()
        {
            this.InitializeComponent();

			// ***
			// *** This is an unusual way to initialize a service to a ViewModel, but since this service
			// *** requires a reference to the WebView this is one way to provide the required reference.
			// *** In this case the WebViewService isn't a traditional Service but more of a shim to provide to better
			// *** separation of View and ViewModel and unit testing of a ViewModel that uses the WebViewService since the
			// *** WebViewService implements the IWebViewService interface that allows for mocking of the service.
			// ***
		   this.ViewModel.WebViewService = new WebViewService(webView);
        }
    }
}
