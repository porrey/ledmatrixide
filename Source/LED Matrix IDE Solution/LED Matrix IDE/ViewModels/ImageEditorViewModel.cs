using System;
using System.Threading.Tasks;
using ImageConverter;
using LedMatrixControl;
using LedMatrixIde.Interfaces;
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
		public ImageEditorViewModel(IUndoService undoService)
		{
			this.UndoService = undoService;

			this.LoadCommand = new DelegateCommand(this.OnLoadCommand, this.OnEnableLoadCommand);
			this.SaveCommand = new DelegateCommand(this.OnSaveCommand, this.OnEnableSaveCommand);
			this.ClearCommand = new DelegateCommand(this.OnClearCommand, this.OnEnableClearCommand);
			this.RotateClockwiseCommand = new DelegateCommand(this.OnRotateClockwiseCommand, this.OnEnableRotateClockwiseCommand);
			this.RotateCounterClockwiseCommand = new DelegateCommand(this.OnRotateCounterClockwiseCommand, this.OnEnableRotateCounterClockwiseCommand);
			this.FlipHorizontalCommand = new DelegateCommand(this.OnFlipHorizontalCommand, this.OnEnableFlipHorizontalCommand);
			this.FlipVerticalCommand = new DelegateCommand(this.OnFlipVerticalCommand, this.OnEnableFlipVerticalCommand);
			this.BuildCommand = new DelegateCommand(this.OnBuildCommand, this.OnEnableBuildCommand);
			this.RedoCommand = new DelegateCommand(this.OnRedoCommand, this.OnEnableRedoCommand);
			this.UndoCommand = new DelegateCommand(this.OnUndoCommand, this.OnEnableUndoCommand);
		}

		protected IUndoService UndoService { get; set; }
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
					this.PixelMatrix.PixelSelected -= this.PixelMatrix_PixelSelected;
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
				// ***
				// *** Cache the old color of this pixel.
				// ***
				Color oldColor = await this.PixelMatrix.GetPixel(e.Row, e.Column);

				if (!this.DrawIsChecked ||
					e.KeyModifiers == VirtualKeyModifiers.Control ||
					e.KeyModifiers == VirtualKeyModifiers.Shift ||
					e.KeyModifiers == VirtualKeyModifiers.Menu)
				{
					if (oldColor != this.PixelMatrix.DefaultBackgroundColor)
					{
						// ***
						// *** Update the pixel.
						// ***
						await this.PixelMatrix.ResetPixel(e.Row, e.Column);

						// ***
						// *** Set up the undo task.
						// ***
						async Task undoAction() { await this.PixelMatrix.SetPixel(e.Row, e.Column, oldColor); }
						async Task redoAction() { await this.PixelMatrix.ResetPixel(e.Row, e.Column); }
						await this.UndoService.AddUndoTask(undoAction, redoAction);
					}
				}
				else
				{
					if (oldColor != this.PixelColor)
					{
						// ***
						// *** Update the pixel.
						// ***
						await this.PixelMatrix.SetPixel(e.Row, e.Column, this.PixelColor);

						// ***
						// *** Set up the undo task.
						// ***
						Color color = this.PixelColor;
						async Task undoAction() { await this.PixelMatrix.SetPixel(e.Row, e.Column, oldColor); }
						async Task redoAction() { await this.PixelMatrix.SetPixel(e.Row, e.Column, color); }
						await this.UndoService.AddUndoTask(undoAction, redoAction);
					}
				}

				this.UndoCommand.RaiseCanExecuteChanged();
				this.RedoCommand.RaiseCanExecuteChanged();
			}
		}

		public DelegateCommand LoadCommand { get; set; }
		public DelegateCommand SaveCommand { get; set; }
		public DelegateCommand ClearCommand { get; set; }
		public DelegateCommand RotateClockwiseCommand { get; set; }
		public DelegateCommand RotateCounterClockwiseCommand { get; set; }
		public DelegateCommand FlipHorizontalCommand { get; set; }
		public DelegateCommand FlipVerticalCommand { get; set; }
		public DelegateCommand BuildCommand { get; set; }
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
				this.BuildCommand.RaiseCanExecuteChanged();
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
				// ***
				// *** Get a copy of the current color matrix.
				// ***
				ColorMatrix oldColorMatrix = await this.PixelMatrix.GetColorMatrix();

				// ***
				// ***
				// ***
				ImageFile imageFile = new ImageFile(file, (uint)this.PixelMatrix.RowCount, (uint)this.PixelMatrix.ColumnCount);
				ColorMatrix colorMatrix = await imageFile.Load();
				await this.PixelMatrix.SetColorMatrix(colorMatrix);

				// ***
				// *** Set up the undo task.
				// ***
				async Task undoAction() { await this.PixelMatrix.SetColorMatrix(oldColorMatrix); }
				async Task redoAction() { await this.PixelMatrix.SetColorMatrix(colorMatrix); }
				await this.UndoService.AddUndoTask(undoAction, redoAction);
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
			// ***
			// *** Get a copy of the current color matrix.
			// ***
			ColorMatrix oldColorMatrix = await this.PixelMatrix.GetColorMatrix();

			// ***
			// *** Clear the matrix.
			// ***
			await this.PixelMatrix.ClearMatrix();

			// ***
			// *** Set up the undo task.
			// ***
			async Task undoAction() { await this.PixelMatrix.SetColorMatrix(oldColorMatrix); }
			async Task redoAction() { await this.PixelMatrix.ClearMatrix(); }
			await this.UndoService.AddUndoTask(undoAction, redoAction);
		}

		public bool OnEnableClearCommand()
		{
			return !this.PickColorIsChecked;
		}

		public async void OnRotateClockwiseCommand()
		{
			// ***
			// *** Get a copy of the current color matrix.
			// ***
			ColorMatrix colorMatrix = await this.PixelMatrix.GetColorMatrix();
			ColorMatrix oldColorMatrix = await colorMatrix.Clone();

			// ***
			// *** Rotate the color matrix and apply it.
			// ***
			await colorMatrix.RotateClockwise();
			await this.PixelMatrix.SetColorMatrix(colorMatrix);

			// ***
			// *** Set up the undo task.
			// ***
			async Task undoAction() { await this.PixelMatrix.SetColorMatrix(oldColorMatrix); }
			async Task redoAction() { await this.PixelMatrix.SetColorMatrix(colorMatrix); }
			await this.UndoService.AddUndoTask(undoAction, redoAction);
		}

		public bool OnEnableRotateClockwiseCommand()
		{
			return !this.PickColorIsChecked;
		}

		public async void OnRotateCounterClockwiseCommand()
		{
			// ***
			// *** Get a copy of the current color matrix.
			// ***
			ColorMatrix colorMatrix = await this.PixelMatrix.GetColorMatrix();
			ColorMatrix oldColorMatrix = await colorMatrix.Clone();

			// ***
			// *** Rotate the color matrix and apply it.
			// ***
			await colorMatrix.RotateCounterClockwise();
			await this.PixelMatrix.SetColorMatrix(colorMatrix);

			// ***
			// *** Set up the undo task.
			// ***
			async Task undoAction() { await this.PixelMatrix.SetColorMatrix(oldColorMatrix); }
			async Task redoAction() { await this.PixelMatrix.SetColorMatrix(colorMatrix); }
			await this.UndoService.AddUndoTask(undoAction, redoAction);
		}

		public bool OnEnableRotateCounterClockwiseCommand()
		{
			return !this.PickColorIsChecked;
		}

		public async void OnFlipHorizontalCommand()
		{
			// ***
			// *** Get a copy of the current color matrix.
			// ***
			ColorMatrix colorMatrix = await this.PixelMatrix.GetColorMatrix();
			ColorMatrix oldColorMatrix = await colorMatrix.Clone();

			// ***
			// *** Flip the color matrix and apply it.
			// ***
			await colorMatrix.FlipHorizontal();
			await this.PixelMatrix.SetColorMatrix(colorMatrix);

			// ***
			// *** Set up the undo task.
			// ***
			async Task undoAction() { await this.PixelMatrix.SetColorMatrix(oldColorMatrix); }
			async Task redoAction() { await this.PixelMatrix.SetColorMatrix(colorMatrix); }
			await this.UndoService.AddUndoTask(undoAction, redoAction);
		}

		public bool OnEnableFlipHorizontalCommand()
		{
			return !this.PickColorIsChecked;
		}

		public async void OnFlipVerticalCommand()
		{
			// ***
			// *** Get a copy of the current color matrix.
			// ***
			ColorMatrix colorMatrix = await this.PixelMatrix.GetColorMatrix();
			ColorMatrix oldColorMatrix = await colorMatrix.Clone();

			// ***
			// *** Flip the color matrix and apply it.
			// ***
			await colorMatrix.FlipVertical();
			await this.PixelMatrix.SetColorMatrix(colorMatrix);

			// ***
			// *** Set up the undo task.
			// ***
			async Task undoAction() { await this.PixelMatrix.SetColorMatrix(oldColorMatrix); }
			async Task redoAction() { await this.PixelMatrix.SetColorMatrix(colorMatrix); }
			await this.UndoService.AddUndoTask(undoAction, redoAction);
		}

		public bool OnEnableFlipVerticalCommand()
		{
			return !this.PickColorIsChecked;
		}

		public void OnBuildCommand()
		{
		}

		public bool OnEnableBuildCommand()
		{
			return false;
		}

		public async void OnRedoCommand()
		{
			await this.UndoService.Redo();
		}

		public bool OnEnableRedoCommand()
		{
			return this.UndoService.CanRedo;
		}

		public async void OnUndoCommand()
		{
			await this.UndoService.Undo();
		}

		public bool OnEnableUndoCommand()
		{
			return this.UndoService.CanRedo;
		}
	}
}
