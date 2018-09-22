using LedMatrixIde.Helpers;
using LedMatrixIde.Services;
using LedMatrixIde.ViewModels;
using Windows.UI.Xaml.Controls;

namespace LedMatrixIde.Views
{
	public sealed partial class ProjectWebViewPage : Page
    {
        private ProjectWebViewViewModel ViewModel => this.DataContext as ProjectWebViewViewModel;

        public ProjectWebViewPage()
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

		public string BackLabel => "Browser_Button_Label_Back".GetLocalized();
		public string BackToolTip => "Browser_Button_ToolTip_Back".GetLocalized();
		public string ForwardLabel => "Browser_Button_Label_Forward".GetLocalized();
		public string ForwardToolTip => "Browser_Button_ToolTip_Forward".GetLocalized();
		public string RefreshLabel => "Browser_Button_Label_Refresh".GetLocalized();
		public string RefreshToolTip => "Browser_Button_ToolTip_Refresh".GetLocalized();
		public string OpenLabel => "Browser_Button_Label_Open".GetLocalized();
		public string OpenToolTip => "Browser_Button_ToolTip_Open".GetLocalized();
	}
}
