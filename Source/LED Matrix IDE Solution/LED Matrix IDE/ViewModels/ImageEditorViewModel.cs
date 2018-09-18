using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using ImageConverter;
using LedMatrixControl;
using LedMatrixIde.Helpers;
using LedMatrixIde.Interfaces;
using Prism.Commands;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

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

		protected const string SelectedColorKey = "SelectedColorKey";
		protected const string ColorMatrixKey = "ColorMatrix";
		protected const string PixelColorKey = "PixelColorKey";

		protected IUndoService UndoService { get; set; }
		protected ColorMatrix CachedColorMatrix { get; set; }

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

					if (this.CachedColorMatrix != null)
					{
						this.PixelMatrix.SetColorMatrixAsync(this.CachedColorMatrix);
					}
				}
			}
		}

		public override async void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
		{
			base.OnNavigatedTo(e, viewModelState);

			if (e.NavigationMode == NavigationMode.Back)
			{
				if (viewModelState.ContainsKey(ImageEditorViewModel.ColorMatrixKey))
				{
					string json = (string)viewModelState[ImageEditorViewModel.ColorMatrixKey];
					this.CachedColorMatrix = await Json.ToObjectAsync<ColorMatrix>(json);
				}

				if (viewModelState.ContainsKey(ImageEditorViewModel.SelectedColorKey))
				{
					this.PixelColor = (Color)viewModelState[ImageEditorViewModel.SelectedColorKey];
				}

				viewModelState.Clear();
			}

			// ***
			// *** Restore pixel color.
			// ***
			this.PixelColor = await ApplicationData.Current.LocalSettings.ReadAsync<Color>(ImageEditorViewModel.SelectedColorKey, Colors.White);

			this.UndoService.TaskAdded += this.UndoService_TaskAdded;
		}

		public override async void OnNavigatingFrom(NavigatingFromEventArgs e, Dictionary<string, object> viewModelState, bool suspending)
		{
			if (e.NavigationMode == NavigationMode.New && !suspending)
			{
				ColorMatrix colorMatrix = await this.PixelMatrix.GetColorMatrixAsync();
				string json = await Json.StringifyAsync(colorMatrix);

				viewModelState[ImageEditorViewModel.ColorMatrixKey] = json;
				viewModelState[ImageEditorViewModel.SelectedColorKey] = this.PixelColor;
			}

			// ***
			// *** Save pixel color.
			// ***
			await ApplicationData.Current.LocalSettings.SaveAsync<Color>(ImageEditorViewModel.SelectedColorKey, this.PixelColor);

			this.UndoService.TaskAdded -= this.UndoService_TaskAdded;
			base.OnNavigatingFrom(e, viewModelState, suspending);
		}

		private void UndoService_TaskAdded(object sender, RoutedEventArgs e)
		{
			this.UndoCommand.RaiseCanExecuteChanged();
			this.RedoCommand.RaiseCanExecuteChanged();
		}

		private async void PixelMatrix_PixelSelected(object sender, PixelSelectedEventArgs e)
		{
			if (this.PickColorIsChecked)
			{
				Color color = await this.PixelMatrix.GetPixelAsync(e.Row, e.Column);
				this.PixelColor = color;
				this.PickColorIsChecked = false;
				this.DrawIsChecked = true;
			}
			else
			{
				// ***
				// *** Cache the old color of this pixel.
				// ***
				Color oldColor = await this.PixelMatrix.GetPixelAsync(e.Row, e.Column);

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
						await this.PixelMatrix.ResetPixelAsync(e.Row, e.Column);

						// ***
						// *** Set up the undo task.
						// ***
						async Task undoAction() { await this.PixelMatrix.SetPixelAsync(e.Row, e.Column, oldColor); }
						async Task redoAction() { await this.PixelMatrix.ResetPixelAsync(e.Row, e.Column); }
						await this.UndoService.AddUndoTask(undoAction, redoAction, $"Reset Pixel [{e.Column}, {e.Row}]");
					}
				}
				else
				{
					if (oldColor != this.PixelColor)
					{
						// ***
						// *** Update the pixel.
						// ***
						await this.PixelMatrix.SetPixelAsync(e.Row, e.Column, this.PixelColor);

						// ***
						// *** Set up the undo task.
						// ***
						Color color = this.PixelColor;
						async Task undoAction() { await this.PixelMatrix.SetPixelAsync(e.Row, e.Column, oldColor); }
						async Task redoAction() { await this.PixelMatrix.SetPixelAsync(e.Row, e.Column, color); }
						await this.UndoService.AddUndoTask(undoAction, redoAction, $"Set Pixel [{e.Column}, {e.Row}, {color.ToString()}]");
					}
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
				ColorMatrix oldColorMatrix = await this.PixelMatrix.GetColorMatrixAsync();

				// ***
				// ***
				// ***
				ImageFile imageFile = new ImageFile(file, (uint)this.PixelMatrix.RowCount, (uint)this.PixelMatrix.ColumnCount);
				ColorMatrix colorMatrix = await imageFile.LoadAsync();
				await this.PixelMatrix.SetColorMatrixAsync(colorMatrix);

				// ***
				// *** Set up the undo task.
				// ***
				async Task undoAction() { await this.PixelMatrix.SetColorMatrixAsync(oldColorMatrix); }
				async Task redoAction() { await this.PixelMatrix.SetColorMatrixAsync(colorMatrix); }
				await this.UndoService.AddUndoTask(undoAction, redoAction, $"Load File [{file.Name}]");
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
				ColorMatrix colorMatrix = await this.PixelMatrix.GetColorMatrixAsync();
				ImageFile imageFile = new ImageFile(storageFile, (uint)this.PixelMatrix.RowCount, (uint)this.PixelMatrix.ColumnCount);
				bool isSaved = await imageFile.SaveAsync(colorMatrix);
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
			ColorMatrix oldColorMatrix = await this.PixelMatrix.GetColorMatrixAsync();

			// ***
			// *** Clear the matrix.
			// ***
			await this.PixelMatrix.ClearMatrixAsync();

			// ***
			// *** Set up the undo task.
			// ***
			async Task undoAction() { await this.PixelMatrix.SetColorMatrixAsync(oldColorMatrix); }
			async Task redoAction() { await this.PixelMatrix.ClearMatrixAsync(); }
			await this.UndoService.AddUndoTask(undoAction, redoAction, "Clear");
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
			ColorMatrix colorMatrix = await this.PixelMatrix.GetColorMatrixAsync();
			ColorMatrix oldColorMatrix = await colorMatrix.CloneAsync();

			// ***
			// *** Rotate the color matrix and apply it.
			// ***
			await colorMatrix.RotateClockwiseAsync();
			await this.PixelMatrix.SetColorMatrixAsync(colorMatrix);

			// ***
			// *** Set up the undo task.
			// ***
			async Task undoAction() { await this.PixelMatrix.SetColorMatrixAsync(oldColorMatrix); }
			async Task redoAction() { await this.PixelMatrix.SetColorMatrixAsync(colorMatrix); }
			await this.UndoService.AddUndoTask(undoAction, redoAction, "Rotate Clockwise");
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
			ColorMatrix colorMatrix = await this.PixelMatrix.GetColorMatrixAsync();
			ColorMatrix oldColorMatrix = await colorMatrix.CloneAsync();

			// ***
			// *** Rotate the color matrix and apply it.
			// ***
			await colorMatrix.RotateCounterClockwiseAsync();
			await this.PixelMatrix.SetColorMatrixAsync(colorMatrix);

			// ***
			// *** Set up the undo task.
			// ***
			async Task undoAction() { await this.PixelMatrix.SetColorMatrixAsync(oldColorMatrix); }
			async Task redoAction() { await this.PixelMatrix.SetColorMatrixAsync(colorMatrix); }
			await this.UndoService.AddUndoTask(undoAction, redoAction, "Rotate Counter-Clockwise");
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
			ColorMatrix colorMatrix = await this.PixelMatrix.GetColorMatrixAsync();
			ColorMatrix oldColorMatrix = await colorMatrix.CloneAsync();

			// ***
			// *** Flip the color matrix and apply it.
			// ***
			await colorMatrix.FlipHorizontalAsync();
			await this.PixelMatrix.SetColorMatrixAsync(colorMatrix);

			// ***
			// *** Set up the undo task.
			// ***
			async Task undoAction() { await this.PixelMatrix.SetColorMatrixAsync(oldColorMatrix); }
			async Task redoAction() { await this.PixelMatrix.SetColorMatrixAsync(colorMatrix); }
			await this.UndoService.AddUndoTask(undoAction, redoAction, "Flip Horizontal");
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
			ColorMatrix colorMatrix = await this.PixelMatrix.GetColorMatrixAsync();
			ColorMatrix oldColorMatrix = await colorMatrix.CloneAsync();

			// ***
			// *** Flip the color matrix and apply it.
			// ***
			await colorMatrix.FlipVerticalAsync();
			await this.PixelMatrix.SetColorMatrixAsync(colorMatrix);

			// ***
			// *** Set up the undo task.
			// ***
			async Task undoAction() { await this.PixelMatrix.SetColorMatrixAsync(oldColorMatrix); }
			async Task redoAction() { await this.PixelMatrix.SetColorMatrixAsync(colorMatrix); }
			await this.UndoService.AddUndoTask(undoAction, redoAction, "Flip Vertical");
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
			return this.UndoService.CanUndo;
		}
	}
}
