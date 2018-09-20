using ImageConverter;
using LedMatrixIde.Helpers;
using LedMatrixIde.ViewModels;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace LedMatrixIde.Views
{
	public partial class ImageEditorPage : Page
	{
		private ImageEditorViewModel ViewModel => this.DataContext as ImageEditorViewModel;

		public ImageEditorPage()
		{
			this.InitializeComponent();

			// ***
			// *** Not the best way to handle communication between the
			// *** view and the view model, but for now
			// *** this is how the pixel changes are communicated
			// *** back and forth. This may get replaced with Prism
			// *** Events.
			// ***
			this.LedMatrix.PixelSelected += this.ViewModel.PixelMatrix_PixelSelected;
			this.ViewModel.Matrix.PixelChanged += this.Matrix_PixelChanged;
			//this.Matrix.ApplyTemplate();
			//this.ViewModel.PixelMatrix = this.Matrix;
		}

		private async void Matrix_PixelChanged(object sender, PixelChangedEventArgs e)
		{
			Color color = e.NewItem;

			// ***
			// *** Any color with a Alpha of 0 is considered a "clear" pixel, but
			// *** have an alpha channel of 0 with cause the mouse events to not fire.
			// ***
			if (color.A == 0)
			{
				color = this.LedMatrix.DefaultBackgroundColor;
			}

			await this.LedMatrix.SetPixelAsync(e.Row, e.Column, color);
		}

		public string ProjectName => "ImageEditor_ProjectName".GetLocalized();
		public string LoadButtonToolTip => "ImageEditor_ToolTip_LoadButton".GetLocalized();
		public string SaveButtonToolTip => "ImageEditor_ToolTip_SaveButton".GetLocalized();
		public string UndoButtonToolTip => "ImageEditor_ToolTip_UndoButton".GetLocalized();
		public string RedoButtonToolTip => "ImageEditor_ToolTip_RedoButton".GetLocalized();
		public string DrawButtonToolTip => "ImageEditor_ToolTip_DrawButton".GetLocalized();
		public string SandButtonToolTip => "ImageEditor_ToolTip_SandButton".GetLocalized();
		public string EraseButtonToolTip => "ImageEditor_ToolTip_EraseButton".GetLocalized();
		public string ColorButtonToolTip => "ImageEditor_ToolTip_ColorButton".GetLocalized();
		public string PickColorButtonToolTip => "ImageEditor_ToolTip_PickColorButton".GetLocalized();
		public string RotateCounterClockwiseButtonToolTip => "ImageEditor_ToolTip_RotateCounterClockwiseButton".GetLocalized();
		public string RotateClockwiseButtonToolTip => "ImageEditor_ToolTip_RotateClockwiseButton".GetLocalized();
		public string FlipHorizontalButtonToolTip => "ImageEditor_ToolTip_FlipHorizontalButton".GetLocalized();
		public string FlipVerticalButtonToolTip => "ImageEditor_ToolTip_FlipVerticalButton".GetLocalized();
		public string ClearButtonToolTip => "ImageEditor_ToolTip_ClearButton".GetLocalized();
		public string BuildButtonToolTip => "ImageEditor_ToolTip_BuildButton".GetLocalized();
		public string OutputButtonToolTip => "ImageEditor_ToolTip_OutputButton".GetLocalized();
		public string BackgroundColorButtonToolTip => "ImageEditor_ToolTip_BackgroundColorButtonToolTip";
	}
}
