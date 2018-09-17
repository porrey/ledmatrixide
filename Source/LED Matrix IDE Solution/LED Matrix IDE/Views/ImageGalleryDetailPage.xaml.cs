using System;

using LedMatrixIde.Models;
using LedMatrixIde.ViewModels;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace LedMatrixIde.Views
{
    public sealed partial class ImageGalleryDetailPage : Page
    {
        public ImageGalleryDetailViewModel ViewModel => DataContext as ImageGalleryDetailViewModel;

        public ImageGalleryDetailPage()
        {
            InitializeComponent();
            ViewModel.SetImage(previewImage);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await ViewModel.InitializeAsync(e.Parameter as string, e.NavigationMode);
            showFlipView.Begin();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                previewImage.Visibility = Visibility.Visible;
                ViewModel.SetAnimation();
            }
        }

        private void OnShowFlipViewCompleted(object sender, object e) => flipView.Focus(FocusState.Programmatic);

        private void OnPageKeyDown(object sender, KeyRoutedEventArgs e)
        {
            ViewModel.HandleKeyDown(e);
        }
    }
}
