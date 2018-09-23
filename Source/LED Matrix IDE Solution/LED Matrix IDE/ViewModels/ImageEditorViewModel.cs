// Copyright © 2018 Daniel Porrey. All Rights Reserved.
//
// This file is part of the LED Matrix IDE Solution project.
// 
// The LED Matrix IDE Solution is free software: you can redistribute it
// and/or modify it under the terms of the GNU General Public License as
// published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
// 
// The LED Matrix IDE Solution is distributed in the hope that it will
// be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with the LED Matrix IDE Solution. If not, 
// see http://www.gnu.org/licenses/.
//
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CodeBuilder;
using CodeBuilder.Models;
using ImageManager;
using LedMatrixControl;
using LedMatrixIde.Decorators;
using LedMatrixIde.Helpers;
using LedMatrixIde.Interfaces;
using LedMatrixIde.Models;
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
	public enum DrawMode
	{
		None,
		Draw,
		Sand,
		Erase,
		PickColor,
		EraseColor
	}

	public class ImageEditorViewModel : ViewModelBase
	{
		public ImageEditorViewModel(IUndoService undoService, IBuildService buildService, IPixelEventService pixelEventService)
		{
			this.UndoService = undoService;
			this.BuildService = buildService;
			this.PixelEventService = pixelEventService;

			// ***
			// *** Group these property so that only one of them
			// *** can be true at any given time.
			// ***
			this.Group = new BooleanPropertyGroup
			(
				(nameof(this.DrawIsChecked), (b) => { this.DrawIsChecked = b; }
			),
				(nameof(this.SandIsChecked), (b) => { this.SandIsChecked = b; }
			),
				(nameof(this.EraseIsChecked), (b) => { this.EraseIsChecked = b; }
			),
				(nameof(this.EraseColorIsChecked), (b) => { this.EraseColorIsChecked = b; }
			),
				(nameof(this.PickColorIsChecked), (b) => { this.PickColorIsChecked = b; }
			)
			);

			// ***
			// *** Wire up events for when the user interacts with the LED Matrix.
			// ***
			this.PixelEventService.PixelSelected += this.PixelMatrix_PixelSelected;

			// ***
			// *** Wire up the event to capture changes to the ColorMatrix.
			// ***
			this.ColorMatrix.PixelChanged += this.ColorMatrix_PixelChanged;

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
			this.CloseOutputCommand = new DelegateCommand(this.OnCloseOutputCommand, this.OnEnableCloseOutputCommand);
		}

		/// <summary>
		/// Defines the size of our LED matrix.
		/// </summary>
		public const uint DefaultRowCount = 64;
		public uint RowCount => ImageEditorViewModel.DefaultRowCount;
		public const uint DefaultColumnCount = 64;
		public uint ColumnCount => ImageEditorViewModel.DefaultColumnCount;

		/// <summary>
		/// Keys used for session state and application settings.
		/// </summary>
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
		public ColorMatrix ColorMatrix { get; } = new ColorMatrix(ImageEditorViewModel.DefaultRowCount, ImageEditorViewModel.DefaultColumnCount);

		/// <summary>
		/// Track changes as a list of Tasks that can be execute to Undo or Redo changes.
		/// </summary>
		protected IUndoService UndoService { get; set; }

		/// <summary>
		/// Provides the services to covert the image to C++ code
		/// that can be compiled on the Raspberry Pi.
		/// </summary>
		protected IBuildService BuildService { get; set; }

		/// <summary>
		/// Provides decoupled (but direct, as opposed to using the Prims
		/// pub.sub model) communication between the LedMatix (on the UI)
		/// and the ColorMatrix used in the view model.
		/// </summary>
		protected IPixelEventService PixelEventService { get; set; }

		/// <summary>
		/// ICommand objects for button binding.
		/// </summary>
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
		public DelegateCommand CloseOutputCommand { get; set; }

		/// <summary>
		/// Groups properties so that at any given
		/// time one and only one is true. This group is
		/// used for all of the properties that the Command
		/// Bar toggle buttons are bound to.
		/// </summary>
		protected BooleanPropertyGroup Group { get; set; }

		/// <summary>
		/// This group of properties controls command
		/// bar application bar buttons that are not
		/// bound to an ICommand.
		/// </summary>
		public bool SandIsEnabled => !this.UseRandomSand;

		/// <summary>
		/// This group of properties control Command Bar
		/// toggle button states via binding.
		/// </summary>
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
				this.Group.SetItem(nameof(this.PickColorIsChecked), _pickColorIsChecked);
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
				this.Group.SetItem(nameof(this.DrawIsChecked), _drawIsChecked);
			}
		}

		private bool _useRandomSand = false;
		public bool UseRandomSand
		{
			get
			{
				return _useRandomSand;
			}
			set
			{
				this.SetProperty(ref _useRandomSand, value);
				this.RaisePropertyChanged(nameof(this.SandIsEnabled));
			}
		}

		private int _randomSandCount = 320;
		public int RandomSandCount
		{
			get
			{
				return _randomSandCount;
			}
			set
			{
				this.SetProperty(ref _randomSandCount, value);
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
				_sandWasChecked = _sandIsChecked;
				this.SetProperty(ref _sandIsChecked, value);
				this.Group.SetItem(nameof(this.SandIsChecked), _sandIsChecked);
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
				this.Group.SetItem(nameof(this.EraseIsChecked), _eraseIsChecked);
			}
		}

		private bool _eraseColorIsChecked = false;
		public bool EraseColorIsChecked
		{
			get
			{
				return _eraseColorIsChecked;
			}
			set
			{
				this.SetProperty(ref _eraseColorIsChecked, value);
				this.Group.SetItem(nameof(this.EraseColorIsChecked), _eraseColorIsChecked);
			}
		}

		/// <summary>
		/// These properties are used to display the colors
		/// on the Command bar via binding.
		/// </summary>
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
				this.ColorMatrix.BackgroundColor = this.BackgroundColor;
			}
		}

		/// <summary>
		/// Binds the current project name to the view.
		/// </summary>
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

		/// <summary>
		/// Binds the visibility of the output window.
		/// </summary>
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

		/// <summary>
		/// 
		/// </summary>
		private bool _buildIsActive = false;
		protected bool BuildIsActive
		{
			get
			{
				return _buildIsActive;
			}
			set
			{
				_buildIsActive = value;
				this.CloseOutputCommand.RaiseCanExecuteChanged();
			}
		}

		private bool _sandWasChecked = false;

		/// <summary>
		/// Contains the items displayed in the output window.
		/// </summary>
		public ObservableCollection<BuildEventArgs> OutputItems { get; } = new ObservableCollection<BuildEventArgs>();

		public override async void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
		{
			base.OnNavigatedTo(e, viewModelState);

			if (e.NavigationMode == NavigationMode.Back)
			{
				ColorMatrix colorMatrix = await Json.ToObjectAsync<ColorMatrix>(await viewModelState.TryGetValue<string, object, string>(ImageEditorViewModel.ColorMatrixKey, "{}"));
				await this.ColorMatrix.CopyFrom(colorMatrix);

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
				viewModelState[ImageEditorViewModel.ColorMatrixKey] = await Json.StringifyAsync(this.ColorMatrix); ;
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

		private async void PixelMatrix_PixelSelected(object sender, PixelSelectedEventArgs e)
		{
			// ***
			// *** Get the current mode.
			// ***
			DrawMode mode = await this.GetDrawMode(e.KeyModifiers);

			// ***
			// *** Cache the old color of this pixel.
			// ***
			ColorItem selectedItem = await this.ColorMatrix.GetItem(e.Row, e.Column);

			switch (mode)
			{
				case DrawMode.PickColor:
					{
						ColorItem colorItem = await this.ColorMatrix.GetItem(e.Row, e.Column);
						this.PixelColor = colorItem;
						this.PickColorIsChecked = false;

						if (_sandWasChecked)
						{
							this.SandIsChecked = true;
						}
						else
						{
							this.DrawIsChecked = true;
						}
					}
					break;
				case DrawMode.Draw:
					{
						if (selectedItem != this.PixelColor || selectedItem.ItemType != ColorItem.ColorItemType.Pixel)
						{
							// ***
							// *** Update the pixel.
							// ***
							Color color = this.PixelColor;
							await this.ColorMatrix.SetItem(e.Row, e.Column, color, ColorItem.ColorItemType.Pixel);

							// ***
							// *** Set up the undo task.
							// ***
							async Task undoAction() { await this.ColorMatrix.SetItem(e.Row, e.Column, selectedItem); }
							async Task redoAction() { await this.ColorMatrix.SetItem(e.Row, e.Column, color, ColorItem.ColorItemType.Pixel); }
							await this.UndoService.AddUndoTask(undoAction, redoAction, $"Set Pixel [{e.Column}, {e.Row}, {this.PixelColor.ToString()}]");
						}
					}
					break;
				case DrawMode.Erase:
					{
						if (selectedItem != this.BackgroundColor || selectedItem.ItemType != ColorItem.ColorItemType.Background)
						{
							// ***
							// *** Update the pixel.
							// ***
							await this.ColorMatrix.SetItem(e.Row, e.Column, this.BackgroundColor, ColorItem.ColorItemType.Background);

							// ***
							// *** Set up the undo task.
							// ***
							async Task undoAction() { await this.ColorMatrix.SetItem(e.Row, e.Column, selectedItem); }
							async Task redoAction() { await this.ColorMatrix.SetItem(e.Row, e.Column, this.BackgroundColor, ColorItem.ColorItemType.Background); }
							await this.UndoService.AddUndoTask(undoAction, redoAction, $"Clear Pixel [{e.Column}, {e.Row}]");
						}
					}
					break;
				case DrawMode.EraseColor:
					{
						if (selectedItem.ItemType != ColorItem.ColorItemType.Background)
						{
							// ***
							// *** Clone the current matrix for the undo.
							// ***
							ColorMatrix oldColorMatrix = await this.ColorMatrix.CloneAsync();
							await this.ColorMatrix.ReplaceColorAsync(selectedItem, this.BackgroundColor, true);

							// ***
							// *** Set up the undo task.
							// ***
							async Task undoAction() { await this.ColorMatrix.CopyFrom(oldColorMatrix); }
							async Task redoAction() { await this.ColorMatrix.ReplaceColorAsync(selectedItem, this.BackgroundColor, true); }
							await this.UndoService.AddUndoTask(undoAction, redoAction, $"Clear Pixels [{e.Column}, {e.Row}]");
						}
					}
					break;
				case DrawMode.Sand:
					{
						if (selectedItem != this.PixelColor || selectedItem.ItemType != ColorItem.ColorItemType.Sand)
						{
							// ***
							// *** Update the pixel.
							// ***
							Color color = this.PixelColor;
							color.A = 255;
							await this.ColorMatrix.SetItem(e.Row, e.Column, color, ColorItem.ColorItemType.Sand);

							// ***
							// *** Set up the undo task.
							// ***
							async Task undoAction() { await this.ColorMatrix.SetItem(e.Row, e.Column, selectedItem); }
							async Task redoAction() { await this.ColorMatrix.SetItem(e.Row, e.Column, color, ColorItem.ColorItemType.Sand); }
							await this.UndoService.AddUndoTask(undoAction, redoAction, $"Set Sand Pixel [{e.Column}, {e.Row}, {this.PixelColor.ToString()}]");
						}
					}
					break;
			}
		}

		private async void ColorMatrix_PixelChanged(object sender, PixelChangedEventArgs e)
		{
			await this.PixelEventService.PublishPixelChangedEvent(e);
		}

		/// <summary>
		/// Determines the current mode based on the user's selection
		/// of certain items in the tool bar.
		/// </summary>
		/// <returns></returns>
		private Task<DrawMode> GetDrawMode(VirtualKeyModifiers keyModifiers = VirtualKeyModifiers.None)
		{
			DrawMode returnValue = DrawMode.None;

			if (this.EraseIsChecked ||
					keyModifiers == VirtualKeyModifiers.Control ||
					keyModifiers == VirtualKeyModifiers.Shift ||
					keyModifiers == VirtualKeyModifiers.Menu)
			{
				returnValue = DrawMode.Erase;
			}
			else if (this.DrawIsChecked)
			{
				returnValue = DrawMode.Draw;
			}
			else if (this.EraseColorIsChecked)
			{
				returnValue = DrawMode.EraseColor;
			}
			else if (this.PickColorIsChecked)
			{
				returnValue = DrawMode.PickColor;
			}
			else if (this.SandIsChecked)
			{
				returnValue = DrawMode.Sand;
			}

			return Task.FromResult(returnValue);
		}

		#region Command Handlers
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
					ColorMatrix oldColorMatrix = await this.ColorMatrix.CloneAsync();

					// ***
					// ***
					// ***
					await this.ColorMatrix.LoadAsync(file, ImageEditorViewModel.DefaultColumnCount, ImageEditorViewModel.DefaultColumnCount);
					ColorMatrix newMatrix = await this.ColorMatrix.CloneAsync();

					// ***
					// *** Set up the undo task.
					// ***
					async Task undoAction() { await this.ColorMatrix.CopyFrom(oldColorMatrix); }
					async Task redoAction() { await this.ColorMatrix.CopyFrom(newMatrix); }
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
			return !this.PickColorIsChecked;
		}

		public async void OnSaveCommand()
		{
			FileSavePicker fileSave = new FileSavePicker
			{
				SuggestedFileName = this.ProjectName
			};

			fileSave.FileTypeChoices.Add("Image", new string[] { ".png" });

			StorageFile storageFile = await fileSave.PickSaveFileAsync();

			if (storageFile != null)
			{
				await this.ColorMatrix.SaveAsync(storageFile, this.ColorMatrix.Height, this.ColorMatrix.Width);
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
			ColorMatrix oldColorMatrix = await this.ColorMatrix.CloneAsync();

			// ***
			// *** Clear the matrix.
			// ***
			await this.ColorMatrix.Clear(this.BackgroundColor);

			// ***
			// *** Clear the project name.
			// ***
			string projectName = this.ProjectName;
			this.ProjectName = String.Empty;

			// ***
			// *** Set up the undo task.
			// ***
			async Task undoAction() { await this.ColorMatrix.CopyFrom(oldColorMatrix); this.ProjectName = projectName; }
			async Task redoAction() { await this.ColorMatrix.Clear(this.BackgroundColor); this.ProjectName = String.Empty; }
			await this.UndoService.AddUndoTask(undoAction, redoAction, "Clear");
		}

		public bool OnEnableClearCommand()
		{
			return !this.PickColorIsChecked;
		}

		public async void OnRotateClockwiseCommand()
		{
			// ***
			// *** Rotate the color matrix and apply it.
			// ***
			await this.ColorMatrix.RotateClockwiseAsync();

			// ***
			// *** Set up the undo task.
			// ***
			async Task undoAction() { await this.ColorMatrix.RotateCounterClockwiseAsync(); }
			async Task redoAction() { await this.ColorMatrix.RotateClockwiseAsync(); }
			await this.UndoService.AddUndoTask(undoAction, redoAction, "Rotate Clockwise");
		}

		public bool OnEnableRotateClockwiseCommand()
		{
			return !this.PickColorIsChecked;
		}

		public async void OnRotateCounterClockwiseCommand()
		{
			// ***
			// *** Rotate the color matrix and apply it.
			// ***
			await this.ColorMatrix.RotateCounterClockwiseAsync();

			// ***
			// *** Set up the undo task.
			// ***
			async Task undoAction() { await this.ColorMatrix.RotateClockwiseAsync(); }
			async Task redoAction() { await this.ColorMatrix.RotateCounterClockwiseAsync(); }
			await this.UndoService.AddUndoTask(undoAction, redoAction, "Rotate Counter-Clockwise");
		}

		public bool OnEnableRotateCounterClockwiseCommand()
		{
			return !this.PickColorIsChecked;
		}

		public async void OnFlipHorizontalCommand()
		{
			// ***
			// *** Flip the color matrix and apply it.
			// ***
			await this.ColorMatrix.FlipHorizontalAsync();

			// ***
			// *** Set up the undo task.
			// ***
			async Task undoAction() { await this.ColorMatrix.FlipHorizontalAsync(); }
			async Task redoAction() { await this.ColorMatrix.FlipHorizontalAsync(); }
			await this.UndoService.AddUndoTask(undoAction, redoAction, "Flip Horizontal");
		}

		public bool OnEnableFlipHorizontalCommand()
		{
			return !this.PickColorIsChecked;
		}

		public async void OnFlipVerticalCommand()
		{
			// ***
			// *** Flip the color matrix and apply it.
			// ***
			await this.ColorMatrix.FlipVerticalAsync();

			// ***
			// *** Set up the undo task.
			// ***
			async Task undoAction() { await this.ColorMatrix.FlipVerticalAsync(); }
			async Task redoAction() { await this.ColorMatrix.FlipVerticalAsync(); }
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
				try
				{
					this.BuildIsActive = true;
					this.OutputItems.Clear();
					this.ShowOutput = true;

					this.BuildService.BuildEvent += (s, e) =>
					{
						this.OutputItems.Add(e);
						this.CloseOutputCommand.RaiseCanExecuteChanged();
					};

					IBuildProject project = new BuildProject()
					{
						Name = this.ProjectName,
						ColorMatrix = this.ColorMatrix,
						PixelColumns = 12,
						MaskColumns = 24,
						UseRandomSand = this.UseRandomSand,
						RandomSandCount = (uint)this.RandomSandCount
					};

					bool result = await this.BuildService.Build(project, folder);
				}
				catch
				{
				}
				finally
				{
					this.BuildIsActive = false;
				}
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

		public void OnCloseOutputCommand()
		{
			this.ShowOutput = false;
		}

		public bool OnEnableCloseOutputCommand()
		{
			return !_buildIsActive;
		}
		#endregion
	}
}
