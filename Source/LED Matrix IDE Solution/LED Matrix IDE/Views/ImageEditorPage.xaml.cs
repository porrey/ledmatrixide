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
using ImageManager;
using LedMatrixControl;
using LedMatrixIde.Helpers;
using LedMatrixIde.Interfaces;
using LedMatrixIde.ViewModels;
using Microsoft.Practices.ServiceLocation;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace LedMatrixIde.Views
{
	public partial class ImageEditorPage : Page
	{
		public ImageEditorPage()
		{
			this.InitializeComponent();

			// ***
			// *** Get the IPixelEventService instance from the container.
			// ***
			this.PixelEventService = ServiceLocator.Current.GetInstance<IPixelEventService>();

			// ***
			// *** Watch for events from the application indicating that the
			// *** LedMatrix needs to be updated.
			// ***
			this.PixelEventService.PixelChanged += this.Matrix_PixelChanged;

			// ***
			// *** Get events from the LedMatrix when the use interacts with it.
			// ***
			this.LedMatrix.PixelSelected += this.LedMatrix_PixelSelected;
		}

		protected ImageEditorViewModel ViewModel => this.DataContext as ImageEditorViewModel;
		protected IPixelEventService PixelEventService { get; set; }

		private async void Matrix_PixelChanged(object sender, PixelChangedEventArgs e)
		{
			Color color;
			Color borderColor;

			// ***
			// *** Check the type of pixel to be drawn.
			// ***
			if (e.NewItem.ItemType == ColorItem.ColorItemType.Background || e.NewItem.A == 0)
			{
				// ***
				// *** Any color with a Alpha of 0 is considered a "clear" pixel. Having
				// *** an alpha channel of 0 with cause the mouse events not to fire.
				// ***
				color = e.NewItem;
				borderColor = this.LedMatrix.DefaultBorderColor;
			}
			else if (e.NewItem.ItemType == ColorItem.ColorItemType.Sand)
			{
				color = e.NewItem;
				borderColor = Colors.LightPink;
			}
			else if (e.NewItem.ItemType == ColorItem.ColorItemType.Pixel)
			{
				// ***
				// *** Blend the pixel on the background
				// ***
				color = e.NewItem;
				//((Color)e.NewItem).NormalBlendColor( );
				borderColor = this.LedMatrix.DefaultBorderColor;
			}

			await this.LedMatrix.SetPixelAsync(e.Row, e.Column, color, borderColor);
		}

		private async void LedMatrix_PixelSelected(object sender, PixelSelectedEventArgs e)
		{
			await this.PixelEventService.PublishPixelSelectedEvent(e);
		}

		public string ProjectName => "ImageEditor_ProjectName".GetLocalized();
		public string LoadButtonToolTip => "ImageEditor_ToolTip_LoadButton".GetLocalized();
		public string SaveButtonToolTip => "ImageEditor_ToolTip_SaveButton".GetLocalized();
		public string UndoButtonToolTip => "ImageEditor_ToolTip_UndoButton".GetLocalized();
		public string RedoButtonToolTip => "ImageEditor_ToolTip_RedoButton".GetLocalized();
		public string DrawButtonToolTip => "ImageEditor_ToolTip_DrawButton".GetLocalized();
		public string SandButtonToolTip => "ImageEditor_ToolTip_SandButton".GetLocalized();
		public string EraseButtonToolTip => "ImageEditor_ToolTip_EraseButton".GetLocalized();
		public string EraseColorButtonToolTip => "ImageEditor_ToolTip_EraseColorButton".GetLocalized();
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
