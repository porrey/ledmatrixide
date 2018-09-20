using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CodeBuilder;
using ImageConverter;
using LedMatrixControl;
using LedMatrixIde.Decorators;
using LedMatrixIde.Helpers;
using LedMatrixIde.Interfaces;
using Prism.Commands;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI;
using Windows.UI.Popups;
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
			this.ClearOutputCommand = new DelegateCommand(this.OnClearOutputCommand, this.OnEnableClearOutputCommand);
		}

		protected const string SelectedColorKey = "SelectedColorKey";
		protected const string BackgroundColorKey = "BackgroundColorKey";
		protected const string ColorMatrixKey = "ColorMatrix";
		protected const string PixelColorKey = "PixelColorKey";
		protected const string ProjectNameKey = "ProjectNameKey";
		protected const string DrawIsCheckedKey = "DrawIsCheckedKey";
		protected const string SandIsCheckedKey = "SandIsCheckedKey";
		protected const string EraseIsCheckedKey = "EraseIsCheckedKey";

		/// <summary>
		/// This is the primary color matrix used to drive the display
		/// and the code build process.
		/// </summary>
		public ColorMatrix Matrix { get; } = new ColorMatrix(64, 64);

		protected IUndoService UndoService { get; set; }
		protected ColorMatrix CachedColorMatrix { get; set; }

		private IPixelMatrix _ledMatrix = null;
		public IPixelMatrix LedMatrix
		{
			get
			{
				return _ledMatrix;
			}
			set
			{
				if (value == null && this.LedMatrix != null)
				{
					this.LedMatrix.PixelSelected -= this.PixelMatrix_PixelSelected;
				}

				this.SetProperty(ref _ledMatrix, value);

				if (this.LedMatrix != null)
				{
					this.LedMatrix.PixelSelected += this.PixelMatrix_PixelSelected;

					if (this.CachedColorMatrix != null)
					{
						this.CachedColorMatrix.SetColorMatrixAsync(this.LedMatrix).Wait();
					}
				}
			}
		}

		public override async void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
		{
			base.OnNavigatedTo(e, viewModelState);

			if (e.NavigationMode == NavigationMode.Back)
			{
				this.CachedColorMatrix = await Json.ToObjectAsync<ColorMatrix>(await viewModelState.TryGetValue<string, object, string>(ImageEditorViewModel.ColorMatrixKey, "{}"));
				this.PixelColor = await viewModelState.TryGetValue<string, object, Color>(ImageEditorViewModel.SelectedColorKey, Colors.White); ;
				this.ProjectName = await viewModelState.TryGetValue<string, object, string>(ImageEditorViewModel.ProjectNameKey, String.Empty);
				this.DrawIsChecked = await viewModelState.TryGetValue<string, object, bool>(ImageEditorViewModel.DrawIsCheckedKey, true);
				this.SandIsChecked = await viewModelState.TryGetValue<string, object, bool>(ImageEditorViewModel.SandIsCheckedKey, false);
				this.EraseIsChecked = await viewModelState.TryGetValue<string, object, bool>(ImageEditorViewModel.EraseIsCheckedKey, false);

				viewModelState.Clear();
			}

			// ***
			// *** Restore pixel color.
			// ***
			this.PixelColor = await ApplicationData.Current.LocalSettings.ReadAsync<Color>(ImageEditorViewModel.SelectedColorKey, Colors.White);
			this.BackgroundColor = await ApplicationData.Current.LocalSettings.ReadAsync<Color>(ImageEditorViewModel.BackgroundColorKey, Colors.Black);

			this.UndoService.TaskAdded += this.UndoService_TaskAdded;
		}

		public override async void OnNavigatingFrom(NavigatingFromEventArgs e, Dictionary<string, object> viewModelState, bool suspending)
		{
			if (e.NavigationMode == NavigationMode.New && !suspending)
			{
				viewModelState[ImageEditorViewModel.ColorMatrixKey] = await Json.StringifyAsync(await this.LedMatrix.GetColorMatrixAsync()); ;
				viewModelState[ImageEditorViewModel.SelectedColorKey] = this.PixelColor;
				viewModelState[ImageEditorViewModel.ProjectNameKey] = this.ProjectName;
				viewModelState[ImageEditorViewModel.DrawIsCheckedKey] = this.DrawIsChecked;
				viewModelState[ImageEditorViewModel.SandIsCheckedKey] = this.SandIsChecked;
				viewModelState[ImageEditorViewModel.EraseIsCheckedKey] = this.EraseIsChecked;
			}

			// ***
			// *** Save pixel color.
			// ***
			await ApplicationData.Current.LocalSettings.SaveAsync<Color>(ImageEditorViewModel.SelectedColorKey, this.PixelColor);
			await ApplicationData.Current.LocalSettings.SaveAsync<Color>(ImageEditorViewModel.BackgroundColorKey, this.BackgroundColor);

			this.UndoService.TaskAdded -= this.UndoService_TaskAdded;
			base.OnNavigatingFrom(e, viewModelState, suspending);
		}

		private void UndoService_TaskAdded(object sender, RoutedEventArgs e)
		{
			this.UndoCommand.RaiseCanExecuteChanged();
			this.RedoCommand.RaiseCanExecuteChanged();
		}

		public async void PixelMatrix_PixelSelected(object sender, PixelSelectedEventArgs e)
		{
			if (this.PickColorIsChecked)
			{
				ColorItem colorItem = await this.Matrix.GetItem(e.Row, e.Column);
				this.PixelColor = colorItem;
				this.PickColorIsChecked = false;
				this.DrawIsChecked = true;
			}
			else
			{
				// ***
				// *** Cache the old color of this pixel.
				// ***
				ColorItem oldColorItem = await this.Matrix.GetItem(e.Row, e.Column);

				if (this.EraseIsChecked ||
					e.KeyModifiers == VirtualKeyModifiers.Control ||
					e.KeyModifiers == VirtualKeyModifiers.Shift ||
					e.KeyModifiers == VirtualKeyModifiers.Menu)
				{
					if (oldColorItem.ItemType != ColorItem.ColorItemType.Background)
					{
						// ***
						// *** Update the pixel.
						// ***
						await this.Matrix.SetItem(e.Row, e.Column, this.PixelColor);

						// ***
						// *** Set up the undo task.
						// ***
						async Task undoAction() { await this.Matrix.SetItem(e.Row, e.Column, oldColorItem); }
						async Task redoAction() { await this.Matrix.SetItem(e.Row, e.Column, this.BackgroundColor, ColorItem.ColorItemType.Background); }
						await this.UndoService.AddUndoTask(undoAction, redoAction, $"Reset Pixel [{e.Column}, {e.Row}]");
					}
				}
				else
				{
					if (this.DrawIsChecked)
					{
						if (oldColorItem != this.PixelColor || oldColorItem.ItemType != ColorItem.ColorItemType.Pixel)
						{
							// ***
							// *** Update the pixel.
							// ***
							await this.Matrix.SetItem(e.Row, e.Column, this.PixelColor, ColorItem.ColorItemType.Pixel);

							// ***
							// *** Set up the undo task.
							// ***
							async Task undoAction() { await this.Matrix.SetItem(e.Row, e.Column, oldColorItem); }
							async Task redoAction() { await this.Matrix.SetItem(e.Row, e.Column, this.PixelColor, ColorItem.ColorItemType.Pixel); }
							await this.UndoService.AddUndoTask(undoAction, redoAction, $"Set Pixel [{e.Column}, {e.Row}, {this.PixelColor.ToString()}]");
						}
					}
					else
					{
						if (oldColorItem != this.PixelColor || oldColorItem.ItemType != ColorItem.ColorItemType.Sand)
						{
							// ***
							// *** Update the pixel.
							// ***
							await this.Matrix.SetItem(e.Row, e.Column, this.PixelColor, ColorItem.ColorItemType.Sand);

							// ***
							// *** Set up the undo task.
							// ***
							async Task undoAction() { await this.Matrix.SetItem(e.Row, e.Column, oldColorItem); }
							async Task redoAction() { await this.Matrix.SetItem(e.Row, e.Column, this.PixelColor, ColorItem.ColorItemType.Sand); }
							await this.UndoService.AddUndoTask(undoAction, redoAction, $"Set Pixel [{e.Column}, {e.Row}, {this.PixelColor.ToString()}]");
						}
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
		public DelegateCommand ClearOutputCommand { get; set; }

		public bool DrawIsEnabled => !this.PickColorIsChecked;
		public bool SandIsEnabled => !this.PickColorIsChecked;
		public bool EraseIsEnabled => !this.PickColorIsChecked;
		public bool ColorPickerIsEnabled => !this.PickColorIsChecked;
		public bool BackgroundColorPickerIsEnabled => !this.PickColorIsChecked;

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

				this.RaisePropertyChanged(nameof(this.DrawIsEnabled));
				this.RaisePropertyChanged(nameof(this.SandIsEnabled));
				this.RaisePropertyChanged(nameof(this.EraseIsEnabled));
				this.RaisePropertyChanged(nameof(this.ColorPickerIsEnabled));
				this.RaisePropertyChanged(nameof(this.BackgroundColorPickerIsEnabled));

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

				if (this.DrawIsChecked)
				{
					this.SandIsChecked = false;
					this.EraseIsChecked = false;
				}
			}
		}

		private bool _sandIsChecked = false;
		public bool SandIsChecked
		{
			get
			{
				return _sandIsChecked;
			}
			set
			{
				this.SetProperty(ref _sandIsChecked, value);

				if (this.SandIsChecked)
				{
					this.DrawIsChecked = false;
					this.EraseIsChecked = false;
				}
			}
		}

		private bool _eraseIsChecked = false;
		public bool EraseIsChecked
		{
			get
			{
				return _eraseIsChecked;
			}
			set
			{
				this.SetProperty(ref _eraseIsChecked, value);

				if (this.EraseIsChecked)
				{
					this.DrawIsChecked = false;
					this.SandIsChecked = false;
				}
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

		private Color _backgroundColor = Colors.Black;
		public Color BackgroundColor
		{
			get
			{
				return _backgroundColor;
			}
			set
			{
				this.SetProperty(ref _backgroundColor, value);
			}
		}

		private string _projectName = String.Empty;
		public string ProjectName
		{
			get
			{
				return _projectName;
			}
			set
			{
				this.SetProperty(ref _projectName, value);

				this.SaveCommand.RaiseCanExecuteChanged();
				this.BuildCommand.RaiseCanExecuteChanged();
			}
		}

		private bool _showOutput = false;
		public bool ShowOutput
		{
			get
			{
				return _showOutput;
			}
			set
			{
				this.SetProperty(ref _showOutput, value);
			}
		}

		public ObservableCollection<BuildEventArgs> OutputItems { get; } = new ObservableCollection<BuildEventArgs>();

		public async void OnLoadCommand()
		{
			try
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
					// *** Use the filename as the project name.
					// ***
					this.ProjectName = file.DisplayName.Replace(" ", "");

					// ***
					// *** Get a copy of the current color matrix.
					// ***
					ColorMatrix oldColorMatrix = await this.LedMatrix.GetColorMatrixAsync();

					// ***
					// ***
					// ***
					ImageFile imageFile = new ImageFile(file, (uint)this.LedMatrix.RowCount, (uint)this.LedMatrix.ColumnCount);
					ColorMatrix colorMatrix = await imageFile.LoadAsync();
					await colorMatrix.SetColorMatrixAsync(this.LedMatrix);

					// ***
					// *** Set up the undo task.
					// ***
					async Task undoAction() { await oldColorMatrix.SetColorMatrixAsync(this.LedMatrix); }
					async Task redoAction() { await colorMatrix.SetColorMatrixAsync(this.LedMatrix); }
					await this.UndoService.AddUndoTask(undoAction, redoAction, $"Load File [{file.Name}]");
				}
			}
			catch (BadImageFormatException)
			{
				MessageDialog dialog = new MessageDialog("ExceptionImageTooLarge".GetLocalized(), "Exception".GetLocalized());
				await dialog.ShowAsync();
			}
			catch (Exception ex)
			{
				MessageDialog dialog = new MessageDialog(ex.Message, "Exception".GetLocalized());
				await dialog.ShowAsync();
			}
		}

		public bool OnEnableLoadCommand()
		{
			return !this.PickColorIsChecked;// && this.LedMatrix != null;
		}

		public async void OnSaveCommand()
		{
			FileSavePicker fileSave = new FileSavePicker();
			fileSave.SuggestedFileName = this.ProjectName;
			fileSave.FileTypeChoices.Add("Image", new string[] { ".png" });
			StorageFile storageFile = await fileSave.PickSaveFileAsync();

			if (storageFile != null)
			{
				ColorMatrix colorMatrix = await this.LedMatrix.GetColorMatrixAsync();
				ImageFile imageFile = new ImageFile(storageFile, (uint)this.LedMatrix.RowCount, (uint)this.LedMatrix.ColumnCount);
				bool isSaved = await imageFile.SaveAsync(colorMatrix);
			}
		}

		public bool OnEnableSaveCommand()
		{
			return !this.PickColorIsChecked &&
				   !String.IsNullOrWhiteSpace(this.ProjectName);
		}

		public async void OnClearCommand()
		{
			// ***
			// *** Get a copy of the current color matrix.
			// ***
			ColorMatrix oldColorMatrix = await this.LedMatrix.GetColorMatrixAsync();

			// ***
			// *** Clear the matrix.
			// ***
			await this.LedMatrix.ClearAsync();

			// ***
			// *** Set up the undo task.
			// ***
			async Task undoAction() { await oldColorMatrix.SetColorMatrixAsync(this.LedMatrix); }
			async Task redoAction() { await this.LedMatrix.ClearAsync(); }
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
			ColorMatrix colorMatrix = await this.LedMatrix.GetColorMatrixAsync();
			ColorMatrix oldColorMatrix = await colorMatrix.CloneAsync();

			// ***
			// *** Rotate the color matrix and apply it.
			// ***
			await colorMatrix.RotateClockwiseAsync();
			await colorMatrix.SetColorMatrixAsync(this.LedMatrix);

			// ***
			// *** Set up the undo task.
			// ***
			async Task undoAction() { await oldColorMatrix.SetColorMatrixAsync(this.LedMatrix); }
			async Task redoAction() { await colorMatrix.SetColorMatrixAsync(this.LedMatrix); }
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
			ColorMatrix colorMatrix = await this.LedMatrix.GetColorMatrixAsync();
			ColorMatrix oldColorMatrix = await colorMatrix.CloneAsync();

			// ***
			// *** Rotate the color matrix and apply it.
			// ***
			await colorMatrix.RotateCounterClockwiseAsync();
			await colorMatrix.SetColorMatrixAsync(this.LedMatrix);

			// ***
			// *** Set up the undo task.
			// ***
			async Task undoAction() { await oldColorMatrix.SetColorMatrixAsync(this.LedMatrix); }
			async Task redoAction() { await colorMatrix.SetColorMatrixAsync(this.LedMatrix); }
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
			ColorMatrix colorMatrix = await this.LedMatrix.GetColorMatrixAsync();
			ColorMatrix oldColorMatrix = await colorMatrix.CloneAsync();

			// ***
			// *** Flip the color matrix and apply it.
			// ***
			await colorMatrix.FlipHorizontalAsync();
			await colorMatrix.SetColorMatrixAsync(this.LedMatrix);

			// ***
			// *** Set up the undo task.
			// ***
			async Task undoAction() { await oldColorMatrix.SetColorMatrixAsync(this.LedMatrix); }
			async Task redoAction() { await colorMatrix.SetColorMatrixAsync(this.LedMatrix); }
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
			ColorMatrix colorMatrix = await this.LedMatrix.GetColorMatrixAsync();
			ColorMatrix oldColorMatrix = await colorMatrix.CloneAsync();

			// ***
			// *** Flip the color matrix and apply it.
			// ***
			await colorMatrix.FlipVerticalAsync();
			await colorMatrix.SetColorMatrixAsync(this.LedMatrix);

			// ***
			// *** Set up the undo task.
			// ***
			async Task undoAction() { await oldColorMatrix.SetColorMatrixAsync(this.LedMatrix); }
			async Task redoAction() { await colorMatrix.SetColorMatrixAsync(this.LedMatrix); }
			await this.UndoService.AddUndoTask(undoAction, redoAction, "Flip Vertical");
		}

		public bool OnEnableFlipVerticalCommand()
		{
			return !this.PickColorIsChecked;
		}

		public async void OnBuildCommand()
		{
			FolderPicker folderPicker = new FolderPicker()
			{
				SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
				ViewMode = PickerViewMode.Thumbnail,
				CommitButtonText = "ImageEditor_BuildHere".GetLocalized()
			};

			folderPicker.FileTypeFilter.Add("*");

			StorageFolder folder = await folderPicker.PickSingleFolderAsync();

			if (folder != null)
			{
				this.OutputItems.Clear();
				this.ShowOutput = true;

				ColorMatrix colorMatrix = await this.LedMatrix.GetColorMatrixAsync();

				IBuilder codeBuilder = new Builder();
				codeBuilder.BuildEvent += (s, e) =>
				{
					this.OutputItems.Add(e);
					this.ClearOutputCommand.RaiseCanExecuteChanged();
				};

				bool result = await codeBuilder.Build(folder, this.ProjectName, colorMatrix, null);
			}
		}

		public bool OnEnableBuildCommand()
		{
			return !String.IsNullOrWhiteSpace(this.ProjectName);
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

		public void OnClearOutputCommand()
		{
			this.OutputItems.Clear();
			this.ClearOutputCommand.RaiseCanExecuteChanged();
		}

		public bool OnEnableClearOutputCommand()
		{
			return (this.OutputItems.Count() > 0);
		}
	}
}
