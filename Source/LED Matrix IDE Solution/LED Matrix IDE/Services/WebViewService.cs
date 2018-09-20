using System;
using LedMatrixIde.Interfaces;
using Windows.UI.Xaml.Controls;

namespace LedMatrixIde.Services
{
	public class WebViewService : IWebViewService
    {
        private WebView _webView;

        public WebViewService(WebView webViewInstance)
        {
            _webView = webViewInstance;
            _webView.NavigationCompleted += this.WebView_NavigationCompleted;
            _webView.NavigationFailed += this.WebView_NavigationFailed;
        }

        public void Detatch()
        {
            if (_webView != null)
            {
                _webView.NavigationCompleted -= this.WebView_NavigationCompleted;
                _webView.NavigationFailed += this.WebView_NavigationFailed;
            }
        }

        private void WebView_NavigationFailed(object sender, WebViewNavigationFailedEventArgs e)
        {
            NavigationFailed?.Invoke(sender, e);
        }

        private void WebView_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs e)
        {
            NavigationComplete?.Invoke(sender, e);
        }

        public void Refresh()
        {
            _webView?.Refresh();
        }

        public void GoForward()
        {
            _webView?.GoForward();
        }

        public void GoBack()
        {
            _webView?.GoBack();
        }

        public bool CanGoForward => _webView?.CanGoForward == true;

        public bool CanGoBack => _webView?.CanGoBack == true;

        public event EventHandler<WebViewNavigationCompletedEventArgs> NavigationComplete;

        public event EventHandler<WebViewNavigationFailedEventArgs> NavigationFailed;
    }
}
