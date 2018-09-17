using System;
using ImageConverter;
using LedMatrixControl;
using Prism.Commands;
using Prism.Windows.Mvvm;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI;

namespace LedMatrixIde.ViewModels
{
	public class ImageEditorViewModel : ViewModelBase
	{
		public ImageEditorViewModel()
		{
			this.LoadCommand = new DelegateCommand(this.OnLoadCommand, this.OnEnableLoadCommand);
			this.SaveCommand = new DelegateCommand(this.OnSaveCommand, this.OnEnableSaveCommand);
			this.ClearCommand = new DelegateCommand(this.OnClearCommand, this.OnEnableClearCommand);
			this.RotateClockwiseCommand = new DelegateCommand(this.OnRotateClockwiseCommand, this.OnEnableRotateClockwiseCommand);
			this.RotateCounterClockwiseCommand = new DelegateCommand(this.OnRotateCounterClockwiseCommand, this.OnEnableRotateCounterClockwiseCommand);
			this.FlipHorizontalCommand = new DelegateCommand(this.OnFlipHorizontalCommand, this.OnEnableFlipHorizontalCommand);
			this.FlipVerticalCommand = new DelegateCommand(this.OnFlipVerticalCommand, this.OnEnableFlipVerticalCommand);
			this.BuildHorizontalCommand = new DelegateCommand(this.OnBuildHorizontalCommand, this.OnEnableBuildHorizontalCommand);
			this.RedoCommand = new DelegateCommand(this.OnUndoCommand, this.OnEnableUndoCommand);
			this.UndoCommand = new DelegateCommand(this.OnRedoCommand, this.OnEnableRedoCommand);
		}

		public bool DrawIsEnabled => !this.PickColorIsChecked;
		public bool EraseIsEnabled => !this.PickColorIsChecked;
		public bool ColorPickerIsEnabled => !this.PickColorIsChecked;

		private IPixelMatrix _pixelMatrix = null;
		public IPixelMatrix PixelMatrix
		{
			get
			{
				return _pixelMatrix;
			}
			set
			{
				if (value == null && this.PixelMatrix != null)
				{
					this.PixelMatrix.PixelSelected-= this.PixelMatrix_PixelSelected;
				}

				this.SetProperty(ref _pixelMatrix, value);

				if (this.PixelMatrix != null)
				{
					this.PixelMatrix.PixelSelected += this.PixelMatrix_PixelSelected;
				}
			}
		}

		private async void PixelMatrix_PixelSelected(object sender, PixelSelectedEventArgs e)
		{
			if (this.PickColorIsChecked)
			{
				Color color = await this.PixelMatrix.GetPixel(e.Row, e.Column);
				this.PixelColor = color;
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
					await this.PixelMatrix.ResetPixel(e.Row, e.Column);
				}
				else
				{
					await this.PixelMatrix.SetPixel(e.Row, e.Column, this.PixelColor);
				}
			}
		}

		public DelegateCommand LoadCommand { get; set; }
		public DelegateCommand SaveCommand { get; set; }
		public DelegateCommand ClearCommand { get; set; }
		public DelegateCommand RotateClockwiseCommand { get; set; }
		public DelegateCommand RotateCounterClockwiseCommand { get; set; }
		public DelegateCommand FlipHorizontalCommand { get; set; }
		public DelegateCommand FlipVerticalCommand { get; set; }
		public DelegateCommand BuildHorizontalCommand { get; set; }
		public DelegateCommand RedoCommand { get; set; }
		public DelegateCommand UndoCommand { get; set; }

		private bool _pickColorIsChecked = false;
		public bool PickColorIsChecked
		{
			get
			{
				return _pickColorIsChecked;
			}
			set
			{
				this.SetProperty(ref _pickColorIsChecked, value);

				this.LoadCommand.RaiseCanExecuteChanged();
				this.SaveCommand.RaiseCanExecuteChanged();
				this.ClearCommand.RaiseCanExecuteChanged();
				this.RotateClockwiseCommand.RaiseCanExecuteChanged();
				this.RotateCounterClockwiseCommand.RaiseCanExecuteChanged();
				this.FlipHorizontalCommand.RaiseCanExecuteChanged();
				this.FlipVerticalCommand.RaiseCanExecuteChanged();
				this.BuildHorizontalCommand.RaiseCanExecuteChanged();
				this.RedoCommand.RaiseCanExecuteChanged();
				this.UndoCommand.RaiseCanExecuteChanged();
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
				this.SetProperty(ref _drawIsChecked, value);
			}
		}

		private Color _pixelColor = Colors.White;
		public Color PixelColor
		{
			get
			{
				return _pixelColor;
			}
			set
			{
				this.SetProperty(ref _pixelColor, value);
			}
		}

		public async void OnLoadCommand()
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
				ImageFile imageFile = new ImageFile(file, (uint)this.PixelMatrix.RowCount, (uint)this.PixelMatrix.ColumnCount);
				ColorMatrix colorMatrix = await imageFile.Load();
				await this.PixelMatrix.SetColorMatrix(colorMatrix);
			}
		}

		public bool OnEnableLoadCommand()
		{
			return !this.PickColorIsChecked;
		}

		public async void OnSaveCommand()
		{
			FileSavePicker fileSave = new FileSavePicker();
			fileSave.FileTypeChoices.Add("Image", new string[] { ".png" });
			StorageFile storageFile = await fileSave.PickSaveFileAsync();

			if (storageFile != null)
			{
				ColorMatrix colorMatrix = await this.PixelMatrix.GetColorMatrix();
				ImageFile imageFile = new ImageFile(storageFile, (uint)this.PixelMatrix.RowCount, (uint)this.PixelMatrix.ColumnCount);
				bool isSaved = await imageFile.Save(colorMatrix);
			}
		}

		public bool OnEnableSaveCommand()
		{
			return !this.PickColorIsChecked;
		}

		public async void OnClearCommand()
		{
			await this.PixelMatrix.ClearMatrix();
		}

		public bool OnEnableClearCommand()
		{
			return !this.PickColorIsChecked;
		}

		public async void OnRotateClockwiseCommand()
		{
			ColorMatrix colorMatrix = await this.PixelMatrix.GetColorMatrix();
			await colorMatrix.RotateClockwise();
			await this.PixelMatrix.ClearMatrix();
			await this.PixelMatrix.SetColorMatrix(colorMatrix);
		}

		public bool OnEnableRotateClockwiseCommand()
		{
			return !this.PickColorIsChecked;
		}

		public async void OnRotateCounterClockwiseCommand()
		{
			ColorMatrix colorMatrix = await this.PixelMatrix.GetColorMatrix();
			await colorMatrix.RotateCounterClockwise();
			await this.PixelMatrix.ClearMatrix();
			await this.PixelMatrix.SetColorMatrix(colorMatrix);
		}

		public bool OnEnableRotateCounterClockwiseCommand()
		{
			return !this.PickColorIsChecked;
		}

		public async void OnFlipHorizontalCommand()
		{
			ColorMatrix colorMatrix = await this.PixelMatrix.GetColorMatrix();
			await colorMatrix.FlipHorizontal();
			await this.PixelMatrix.ClearMatrix();
			await this.PixelMatrix.SetColorMatrix(colorMatrix);
		}

		public bool OnEnableFlipHorizontalCommand()
		{
			return !this.PickColorIsChecked;
		}

		public async void OnFlipVerticalCommand()
		{
			ColorMatrix colorMatrix = await this.PixelMatrix.GetColorMatrix();
			await colorMatrix.FlipVertical();
			await this.PixelMatrix.ClearMatrix();
			await this.PixelMatrix.SetColorMatrix(colorMatrix);
		}

		public bool OnEnableFlipVerticalCommand()
		{
			return !this.PickColorIsChecked;
		}

		public void OnBuildHorizontalCommand()
		{
		}

		public bool OnEnableBuildHorizontalCommand()
		{
			return false;
		}

		public void OnRedoCommand()
		{
		}

		public bool OnEnableRedoCommand()
		{
			return false;
		}

		public void OnUndoCommand()
		{
		}

		public bool OnEnableUndoCommand()
		{
			return false;
		}
	}
}
