using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ImageConverter;
using LedMatrixControl;
using LedMatrixIde.ViewModels;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LedMatrixIde.Views
{
	public partial class ImageEditorPage : Page, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged = null;
		private ImageEditorViewModel ViewModel => this.DataContext as ImageEditorViewModel;

		public ImageEditorPage()
		{
			this.InitializeComponent();
		}

		public bool LoadIsEnabled => !this.PickColorIsChecked;
		public bool SaveIsEnabled => !this.PickColorIsChecked;
		public bool ClearIsEnabled => !this.PickColorIsChecked;
		public bool ColorPickerIsEnabled => !this.PickColorIsChecked;
		public bool RedoIsEnabled => !this.PickColorIsChecked;
		public bool UndoIsEnabled => !this.PickColorIsChecked;
		public bool DrawIsEnabled => !this.PickColorIsChecked;
		public bool EraseIsEnabled => !this.PickColorIsChecked;
		public bool RotateClockwiseIsEnabled => !this.PickColorIsChecked;
		public bool RotateCounterClockwiseIsEnabled => !this.PickColorIsChecked;
		public bool FlipHorizontalIsEnabled => !this.PickColorIsChecked;
		public bool FlipVerticalIsEnabled => !this.PickColorIsChecked;
		public bool BuildIsEnabled => !this.PickColorIsChecked;

		private bool _pickColorIsChecked = false;
		public bool PickColorIsChecked
		{
			get
			{
				return _pickColorIsChecked;
			}
			set
			{
				_pickColorIsChecked = value;

				this.OnPropertyChanged();
				this.OnPropertyChanged(nameof(this.LoadIsEnabled));
				this.OnPropertyChanged(nameof(this.SaveIsEnabled));
				this.OnPropertyChanged(nameof(this.ClearIsEnabled));
				this.OnPropertyChanged(nameof(this.ColorPickerIsEnabled));
				this.OnPropertyChanged(nameof(this.RedoIsEnabled));
				this.OnPropertyChanged(nameof(this.UndoIsEnabled));
				this.OnPropertyChanged(nameof(this.DrawIsEnabled));
				this.OnPropertyChanged(nameof(this.EraseIsEnabled));
				this.OnPropertyChanged(nameof(this.RotateClockwiseIsEnabled));
				this.OnPropertyChanged(nameof(this.RotateCounterClockwiseIsEnabled));
				this.OnPropertyChanged(nameof(this.FlipHorizontalIsEnabled));
				this.OnPropertyChanged(nameof(this.FlipVerticalIsEnabled));
			}
		}

		private bool _drawIsChecked = true;
		public bool DrawIsChecked
		{
			get
			{
				return _drawIsChecked;
			}
			set
			{
				_drawIsChecked = value;
				this.OnPropertyChanged();
			}
		}

		private async void LoadImage_Click(object sender, RoutedEventArgs e)
		{
			FileOpenPicker openPicker = new FileOpenPicker
			{
				ViewMode = PickerViewMode.Thumbnail,
				SuggestedStartLocation = PickerLocationId.PicturesLibrary
			};

			openPicker.FileTypeFilter.Add(".jpg");
			openPicker.FileTypeFilter.Add(".jpeg");
			openPicker.FileTypeFilter.Add(".png");
			openPicker.FileTypeFilter.Add(".bmp");

			StorageFile file = await openPicker.PickSingleFileAsync();

			if (file != null)
			{
				ImageFile imageFile = new ImageFile(file, (uint)this.Matrix.RowCount, (uint)this.Matrix.ColumnCount);
				ColorMatrix colorMatrix = await imageFile.Load();
				await this.Matrix.SetColorMatrix(colorMatrix);
			}
		}

		private async void Clear_Click(object sender, RoutedEventArgs e)
		{
			await this.Matrix.ClearMatrix();
		}

		private async void SaveImage_Click(object sender, RoutedEventArgs e)
		{
			FileSavePicker fileSave = new FileSavePicker();
			fileSave.FileTypeChoices.Add("Image", new string[] { ".png" });
			StorageFile storageFile = await fileSave.PickSaveFileAsync();

			if (storageFile != null)
			{
				ColorMatrix colorMatrix = await this.Matrix.GetColorMatrix();
				ImageFile imageFile = new ImageFile(storageFile, (uint)this.Matrix.RowCount, (uint)this.Matrix.ColumnCount);
				bool isSaved = await imageFile.Save(colorMatrix);
			}
		}

		private async void Matrix_PixelChanged(object sender, PixelSelectedEventArgs e)
		{
			if (this.PickColorIsChecked)
			{
				Color color = await this.Matrix.GetPixel(e.Row, e.Column);
				this.PixelColor.Color = color;
				this.PickColorIsChecked = false;
				this.DrawIsChecked = true;
			}
			else
			{
				if (!this.DrawIsChecked ||
					e.KeyModifiers == VirtualKeyModifiers.Control ||
					e.KeyModifiers == VirtualKeyModifiers.Shift ||
					e.KeyModifiers == VirtualKeyModifiers.Menu)
				{
					await this.Matrix.ResetPixel(e.Row, e.Column);
				}
				else
				{
					await this.Matrix.SetPixel(e.Row, e.Column, this.PixelColor.Color);
				}
			}
		}

		protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private async void RotateClockwise_Click(object sender, RoutedEventArgs e)
		{
			ColorMatrix colorMatrix = await this.Matrix.GetColorMatrix();
			await colorMatrix.RotateClockwise();
			await this.Matrix.ClearMatrix();
			await this.Matrix.SetColorMatrix(colorMatrix);
		}

		private async void RotateCounterClockwise_Click(object sender, RoutedEventArgs e)
		{
			ColorMatrix colorMatrix = await this.Matrix.GetColorMatrix();
			await colorMatrix.RotateCounterClockwise();
			await this.Matrix.ClearMatrix();
			await this.Matrix.SetColorMatrix(colorMatrix);
		}

		private async void FlipHorizontal_Click(object sender, RoutedEventArgs e)
		{
			ColorMatrix colorMatrix = await this.Matrix.GetColorMatrix();
			await colorMatrix.FlipHorizontal();
			await this.Matrix.ClearMatrix();
			await this.Matrix.SetColorMatrix(colorMatrix);
		}

		private async void FlipVertical_Click(object sender, RoutedEventArgs e)
		{
			ColorMatrix colorMatrix = await this.Matrix.GetColorMatrix();
			await colorMatrix.FlipVertical();
			await this.Matrix.ClearMatrix();
			await this.Matrix.SetColorMatrix(colorMatrix);
		}

		private void Build_Click(object sender, RoutedEventArgs e)
		{
			
		}
	}
}
