﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace LedMatrixControl
{
	public class PixelMatrix : Control
	{
		public event EventHandler<PixelSelectedEventArgs> PixelSelected = null;

		public PixelMatrix()
		{
			this.DefaultStyleKey = typeof(PixelMatrix);

			this.PointerMoved += this.UiElement_PointerMoved;
			this.PointerPressed += this.UiElement_PointerPressed;
			this.PointerReleased += this.UiElement_PointerReleased;

			this.PixelBackground = new SolidColorBrush(this.DefaultBackgroundColor);
			this.PixelBorder = new SolidColorBrush(this.DefaultBorderColor);

			this.SizeChanged += this.PixelMatrix_SizeChanged;
		}

		public static readonly DependencyProperty RowCountProperty = DependencyProperty.Register("RowCount", typeof(uint), typeof(PixelMatrix), null);
		public static readonly DependencyProperty ColumnCountProperty = DependencyProperty.Register("ColumnCount", typeof(uint), typeof(PixelMatrix), null);
		public static readonly DependencyProperty PixelBackgroundProperty = DependencyProperty.Register("PixelBackground", typeof(Brush), typeof(PixelMatrix), null);
		public static readonly DependencyProperty PixelBorderProperty = DependencyProperty.Register("PixelBorder", typeof(Brush), typeof(PixelMatrix), null);

		protected Border[,] Cells { get; set; }
		protected bool PointerIsPressed = false;
		protected Grid MainGrid { get; set; }
		protected uint PreviousRow { get; set; } = 0;
		protected uint PreviousColumn { get; set; } = 0;
		protected VirtualKeyModifiers PreviousKeyModifiers { get; set; } = VirtualKeyModifiers.None;

		public Color DefaultBackgroundColor => Color.FromArgb(255, 0, 0, 0);
		public Color DefaultBorderColor => Color.FromArgb(255, 0, 0, 0);

		public uint RowCount
		{
			get
			{
				return (uint)this.GetValue(RowCountProperty);
			}
			set
			{
				this.SetValue(RowCountProperty, value);
			}
		}

		public uint ColumnCount
		{
			get
			{
				return (uint)this.GetValue(ColumnCountProperty);
			}
			set
			{
				this.SetValue(ColumnCountProperty, value);
			}
		}

		public Brush PixelBackground
		{
			get
			{
				return (Brush)this.GetValue(PixelBackgroundProperty);
			}
			set
			{
				this.SetValue(PixelBackgroundProperty, value);
			}
		}

		public Brush PixelBorder
		{
			get
			{
				return (Brush)this.GetValue(PixelBorderProperty);
			}
			set
			{
				this.SetValue(PixelBorderProperty, value);
			}
		}

		public Task<Color> GetPixelAsync(uint row, uint column)
		{
			Color color = ((SolidColorBrush)this.Cells[row, column].Background).Color;
			return Task.FromResult(color);
		}

		public async Task SetPixelAsync(uint row, uint column, Color backgroundColor)
		{
			await this.SetPixelAsync(row, column, backgroundColor, this.DefaultBorderColor);
		}

		public Task SetPixelAsync(uint row, uint column, Color backgroundColor, Color borderColor)
		{
			this.Cells[row, column].Background = new SolidColorBrush(backgroundColor);
			this.Cells[row, column].BorderBrush = new SolidColorBrush(borderColor);

			return Task.FromResult(0);
		}

		public async Task ClearAsync()
		{
			for (uint row = 0; row < this.RowCount; row++)
			{
				for (uint column = 0; column < this.ColumnCount; column++)
				{

					await this.SetPixelAsync(row, column, this.DefaultBackgroundColor, this.DefaultBorderColor);
				}
			}
		}

		protected override void OnApplyTemplate()
		{
			if (this.GetTemplateChild("PART_Grid") is Grid grid)
			{
				this.MainGrid = grid;
				this.Cells = new Border[this.RowCount, this.ColumnCount];

				// ***
				// *** Add the pixel rows.
				// ***
				for (uint i = 0; i < this.RowCount; i++)
				{
					RowDefinition row = new RowDefinition
					{
						Height = new GridLength(1, GridUnitType.Star)
					};

					this.MainGrid.RowDefinitions.Add(row);
				}

				// ***
				// *** Add the pixel columns.
				// ***
				for (uint i = 0; i < this.ColumnCount; i++)
				{
					ColumnDefinition column = new ColumnDefinition
					{
						Width = new GridLength(1, GridUnitType.Star)
					};

					this.MainGrid.ColumnDefinitions.Add(column);
				}

				// ***
				// *** Create the controls for each pixel.
				// ***
				for (uint row = 0; row < this.RowCount; row++)
				{
					for (uint column = 0; column < this.ColumnCount; column++)
					{
						this.Cells[row, column] = new Border()
						{
							BorderThickness = new Thickness(.5, .5, .5, .5),
							Margin = new Thickness(.5, .5, .5, .5),
							BorderBrush = this.PixelBorder,
							Background = this.PixelBackground,
						};

						this.Cells[row, column].SetValue(Grid.ColumnProperty, column);
						this.Cells[row, column].SetValue(Grid.RowProperty, row);

						this.MainGrid.Children.Add(this.Cells[row, column]);
					}
				}
			}
		}

		private Task<(uint Row, uint Column)> GetPixelCoordinates(Border border)
		{
			(uint Row, uint Column) returnValue = (0, 0);

			returnValue.Row = Convert.ToUInt32(border.GetValue(Grid.RowProperty));
			returnValue.Column = Convert.ToUInt32(border.GetValue(Grid.ColumnProperty));

			return Task.FromResult(returnValue);
		}

		private void OnPixelChanged(PixelSelectedEventArgs e)
		{
			this.PixelSelected?.Invoke(this, e);
		}

		private async Task ProcessPointer(PointerRoutedEventArgs e)
		{
			PointerPoint point = e.GetCurrentPoint(null);
			IEnumerable<UIElement> elements = VisualTreeHelper.FindElementsInHostCoordinates(point.Position, this.MainGrid);

			if (elements.Count() > 0)
			{
				// ***
				// *** The top UI Element will be the pixel
				// ***
				if (elements.FirstOrDefault() is Border border)
				{
					(uint row, uint column) = await this.GetPixelCoordinates(border);

					if (this.PreviousRow != row ||
						this.PreviousColumn != column ||
						this.PreviousKeyModifiers != e.KeyModifiers)
					{
						this.OnPixelChanged(new PixelSelectedEventArgs(row, column, e.KeyModifiers));

						this.PreviousRow = row;
						this.PreviousColumn = column;
						this.PreviousKeyModifiers = e.KeyModifiers;
					}
				}
			}
		}

		private async void UiElement_PointerPressed(object sender, PointerRoutedEventArgs e)
		{
			this.CapturePointer(e.Pointer);
			this.PointerIsPressed = true;
			await this.ProcessPointer(e);
		}

		private void UiElement_PointerReleased(object sender, PointerRoutedEventArgs e)
		{
			this.PointerIsPressed = false;
			this.ReleasePointerCapture(e.Pointer);
		}

		private async void UiElement_PointerMoved(object sender, PointerRoutedEventArgs e)
		{
			if (this.PointerIsPressed)
			{
				await this.ProcessPointer(e);
			}
		}

		private void PixelMatrix_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (this.Parent is Grid grid)
			{
				if (e.NewSize.Width != e.NewSize.Height)
				{
					if (e.NewSize.Width > e.NewSize.Height)
					{
						if (e.NewSize.Width > grid.ActualHeight)
						{
							this.Width = e.NewSize.Height - this.Margin.Left - this.Margin.Right;
						}
						else
						{
							this.Height = e.NewSize.Width - this.Margin.Top - this.Margin.Bottom;
						}
					}
					else
					{
						if (e.NewSize.Height > grid.ActualWidth)
						{
							this.Height = e.NewSize.Width - this.Margin.Top - this.Margin.Bottom;
						}
						else
						{
							this.Width = e.NewSize.Height - this.Margin.Left - this.Margin.Right;
						}
					}
				}
			}
		}
	}
}
