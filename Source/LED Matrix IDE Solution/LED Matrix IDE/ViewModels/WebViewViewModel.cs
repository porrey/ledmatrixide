using System;
using System.Windows.Input;
using LedMatrixIde.Interfaces;
using Prism.Commands;
using Prism.Windows.Mvvm;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LedMatrixIde.ViewModels
{
	public abstract class WebViewViewModel : ViewModelBase
    {
        public virtual string DefaultUrl => "";

        public WebViewViewModel()
        {
            this.IsLoading = true;
            this.Source = new Uri(this.DefaultUrl);

            this.BrowserBackCommand = new DelegateCommand(() => _webViewService?.GoBack(), () => _webViewService?.CanGoBack ?? false);
            this.BrowserForwardCommand = new DelegateCommand(() => _webViewService?.GoForward(), () => _webViewService?.CanGoForward ?? false);
            this.RefreshCommand = new DelegateCommand(() => _webViewService?.Refresh());
            this.RetryCommand = new DelegateCommand(this.Retry);
            this.OpenInBrowserCommand = new DelegateCommand(async () => await Windows.System.Launcher.LaunchUriAsync(Source));
        }

        private Uri _source;
        public Uri Source
        {
            get
            {
                return _source;
            }
            set
            {
                this.SetProperty(ref _source, value);
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }

            set
            {
                if (value)
                {
                    this.IsShowingFailedMessage = false;
                }

                this.SetProperty(ref _isLoading, value);
                this.IsLoadingVisibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private Visibility _isLoadingVisibility;
        public Visibility IsLoadingVisibility
        {
            get
            {
                return _isLoadingVisibility;
            }

            set
            {
                this.SetProperty(ref _isLoadingVisibility, value);
            }
        }

        private bool _isShowingFailedMessage;
        public bool IsShowingFailedMessage
        {
            get
            {
                return _isShowingFailedMessage;
            }

            set
            {
                if (value)
                {
                    this.IsLoading = false;
                }

                this.SetProperty(ref _isShowingFailedMessage, value);
                this.FailedMesageVisibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private Visibility _failedMessageVisibility;
        public Visibility FailedMesageVisibility
        {
            get
            {
                return _failedMessageVisibility;
            }

            set
            {
                this.SetProperty(ref _failedMessageVisibility, value);
            }
        }

        private IWebViewService _webViewService;
        public IWebViewService WebViewService
        {
            get
            {
                return _webViewService;
            }
            set
            {
                if (_webViewService != null)
                {
                    _webViewService.NavigationComplete -= this.WebViewService_NavigationComplete;
                    _webViewService.NavigationFailed -= this.WebViewService_NavigationFailed;
                }

                _webViewService = value;
                _webViewService.NavigationComplete += this.WebViewService_NavigationComplete;
                _webViewService.NavigationFailed += this.WebViewService_NavigationFailed;
            }
        }

        public ICommand BrowserBackCommand { get; }
        public ICommand BrowserForwardCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand RetryCommand { get; }
        public ICommand OpenInBrowserCommand { get; }

        private void WebViewService_NavigationFailed(object sender, WebViewNavigationFailedEventArgs e)
        {
            this.NavFailed(e);
        }

        private void WebViewService_NavigationComplete(object sender, WebViewNavigationCompletedEventArgs e)
        {
            this.NavCompleted(e);
        }

        private void NavCompleted(WebViewNavigationCompletedEventArgs e)
        {
            this.IsLoading = false;
            this.RaisePropertyChanged(nameof(this.BrowserBackCommand));
            this.RaisePropertyChanged(nameof(this.BrowserForwardCommand));
        }

        private void NavFailed(WebViewNavigationFailedEventArgs e)
        {
            // Use `e.WebErrorStatus` to vary the displayed message based on the error reason
            this.IsShowingFailedMessage = true;
        }

        private void Retry()
        {
            this.IsShowingFailedMessage = false;
            this.IsLoading = true;

            _webViewService?.Refresh();
        }
    }
}
