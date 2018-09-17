using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using LedMatrixIde.Helpers;
using LedMatrixIde.Models;
using LedMatrixIde.Services;
using LedMatrixIde.Views;

using Prism.Commands;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;

using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace LedMatrixIde.ViewModels
{
    public class ImageGalleryViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly ISampleDataService _sampleDataService;

        public const string ImageGallerySelectedIdKey = "ImageGallerySelectedIdKey";
        public const string ImageGalleryAnimationOpen = "ImageGallery_AnimationOpen";
        public const string ImageGalleryAnimationClose = "ImageGallery_AnimationClose";

        private ObservableCollection<SampleImage> _source;
        private ICommand _itemSelectedCommand;
        private GridView _imagesGridView;

        public ObservableCollection<SampleImage> Source
        {
            get => _source;
            set => SetProperty(ref _source, value);
        }

        public ICommand ItemSelectedCommand => _itemSelectedCommand ?? (_itemSelectedCommand = new DelegateCommand<ItemClickEventArgs>(OnsItemSelected));

        public ImageGalleryViewModel(INavigationService navigationServiceInstance, ISampleDataService sampleDataServiceInstance)
        {
            _navigationService = navigationServiceInstance;

            // TODO WTS: Replace this with your actual data
            _sampleDataService = sampleDataServiceInstance;
            Source = _sampleDataService.GetGallerySampleData();
        }

        public void Initialize(GridView imagesGridView)
        {
            _imagesGridView = imagesGridView;
        }

        public async Task LoadAnimationAsync()
        {
            var selectedImageId = await ApplicationData.Current.LocalSettings.ReadAsync<string>(ImageGallerySelectedIdKey);
            if (!string.IsNullOrEmpty(selectedImageId))
            {
                var animation = ConnectedAnimationService.GetForCurrentView().GetAnimation(ImageGalleryAnimationClose);
                if (animation != null)
                {
                    var item = _imagesGridView.Items.FirstOrDefault(i => ((SampleImage)i).ID == selectedImageId);
                    _imagesGridView.ScrollIntoView(item);
                    await _imagesGridView.TryStartConnectedAnimationAsync(animation, item, "galleryImage");
                }

                ApplicationData.Current.LocalSettings.SaveString(ImageGallerySelectedIdKey, string.Empty);
            }
        }

        private void OnsItemSelected(ItemClickEventArgs args)
        {
            var selected = args.ClickedItem as SampleImage;
            _imagesGridView.PrepareConnectedAnimation(ImageGalleryAnimationOpen, selected, "galleryImage");
            _navigationService.Navigate(PageTokens.ImageGalleryDetailPage, selected.ID);
        }
    }
}
