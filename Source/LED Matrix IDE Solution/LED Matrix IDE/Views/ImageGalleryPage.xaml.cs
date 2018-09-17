using System;

using LedMatrixIde.ViewModels;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace LedMatrixIde.Views
{
    public sealed partial class ImageGalleryPage : Page
    {
        private ImageGalleryViewModel ViewModel => DataContext as ImageGalleryViewModel;

        public ImageGalleryPage()
        {
            InitializeComponent();
            ViewModel.Initialize(gridView);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                await ViewModel.LoadAnimationAsync();
            }
        }
    }
}
