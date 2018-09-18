using LedMatrixIde.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace LedMatrixIde.Views
{
	public partial class ImageEditorPage : Page
	{
		private ImageEditorViewModel ViewModel => this.DataContext as ImageEditorViewModel;

		public ImageEditorPage()
		{
			this.InitializeComponent();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			if (this.ViewModel != null)
			{
				this.Matrix.ApplyTemplate();
				this.ViewModel.PixelMatrix = this.Matrix;
			}

			base.OnNavigatedTo(e);
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			base.OnNavigatedFrom(e);

			if (this.ViewModel != null)
			{
				this.ViewModel.PixelMatrix = null;
			}
		}
	}
}
