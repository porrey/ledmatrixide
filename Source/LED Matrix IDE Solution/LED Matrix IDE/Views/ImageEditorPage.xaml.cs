using LedMatrixIde.Helpers;
using LedMatrixIde.ViewModels;
using Windows.UI.Xaml.Controls;

namespace LedMatrixIde.Views
{
	public partial class ImageEditorPage : Page
	{
		private ImageEditorViewModel ViewModel => this.DataContext as ImageEditorViewModel;

		public ImageEditorPage()
		{
			this.InitializeComponent();

			this.Matrix.ApplyTemplate();
			this.ViewModel.PixelMatrix = this.Matrix;
		}

		public string ProjectName => "ImageEditor_ProjectName".GetLocalized();
		public string LoadButtonToolTip => "ImageEditor_ToolTip_LoadButton".GetLocalized();
		public string SaveButtonToolTip => "ImageEditor_ToolTip_SaveButton".GetLocalized();
		public string UndoButtonToolTip => "ImageEditor_ToolTip_UndoButton".GetLocalized();
		public string RedoButtonToolTip => "ImageEditor_ToolTip_RedoButton".GetLocalized();
		public string DrawButtonToolTip => "ImageEditor_ToolTip_DrawButton".GetLocalized();
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
