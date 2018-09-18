using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageConverter;
using Windows.System;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace LedMatrixControl
{
	public class PixelMatrix : Control, IPixelMatrix
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

		public static readonly DependencyProperty RowCountProperty = DependencyProperty.Register("RowCount", typeof(int), typeof(PixelMatrix), null);
		public static readonly DependencyProperty ColumnCountProperty = DependencyProperty.Register("ColumnCount", typeof(int), typeof(PixelMatrix), null);
		public static readonly DependencyProperty PixelBackgroundProperty = DependencyProperty.Register("PixelBackground", typeof(Brush), typeof(PixelMatrix), null);
		public static readonly DependencyProperty PixelBorderProperty = DependencyProperty.Register("PixelBorder", typeof(Brush), typeof(PixelMatrix), null);

		protected Border[,] Cells { get; set; }
		protected bool PointerIsPressed = false;
		protected Grid MainGrid { get; set; }
		protected int PreviousRow { get; set; } = 0;
		protected int PreviousColumn { get; set; } = 0;
		protected VirtualKeyModifiers PreviousKeyModifiers { get; set; } = VirtualKeyModifiers.None;

		public Color DefaultBackgroundColor => Color.FromArgb(255, 0, 0, 0);
		public Color DefaultBorderColor => Color.FromArgb(255, 0, 0, 0);

		public int RowCount
		{
			get
			{
				return (int)this.GetValue(RowCountProperty);
			}
			set
			{
				this.SetValue(RowCountProperty, value);
			}
		}

		public int ColumnCount
		{
			get
			{
				return (int)this.GetValue(ColumnCountProperty);
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

		public Task<Color> GetPixelAsync(int row, int column)
		{
			Color color = ((SolidColorBrush)this.Cells[row, column].Background).Color;
			return Task.FromResult(color);
		}

		public Task SetPixelAsync(int row, int column, Color color)
		{
			this.Cells[row, column].Background = new SolidColorBrush(color);
			this.Cells[row, column].BorderBrush = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
			return Task.FromResult(0);
		}

		public Task ResetPixelAsync(int row, int column)
		{
			this.Cells[row, column].Background = this.PixelBackground;
			this.Cells[row, column].BorderBrush = this.PixelBorder;
			return Task.FromResult(0);
		}

		public async Task ClearMatrixAsync()
		{
			for (int row = 0; row < this.RowCount; row++)
			{
				for (int column = 0; column < this.ColumnCount; column++)
				{

					await this.ResetPixelAsync(row, column);
				}
			}
		}

		public async Task<ColorMatrix> GetColorMatrixAsync()
		{
			ColorMatrix returnValue = new ColorMatrix((uint)this.RowCount, (uint)this.ColumnCount);

			for (int row = 0; row < this.RowCount; row++)
			{
				for (int column = 0; column < this.ColumnCount; column++)
				{
					returnValue.Colors[row, column] = await this.GetPixelAsync(row, column);
				}
			}

			return returnValue;
		}

		public async Task SetColorMatrixAsync(ColorMatrix colorMatrix)
		{
			for (int row = 0; row < this.RowCount; row++)
			{
				for (int column = 0; column < this.ColumnCount; column++)
				{
					Color color = colorMatrix.Colors[row, column];

					if (color.A > 0)
					{
						await this.SetPixelAsync(row, column, color);
					}
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
				for (int i = 0; i < this.RowCount; i++)
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
				for (int i = 0; i < this.ColumnCount; i++)
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
				for (int row = 0; row < this.RowCount; row++)
				{
					for (int column = 0; column < this.ColumnCount; column++)
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

		protected void OnPixelChanged(PixelSelectedEventArgs e)
		{
			this.PixelSelected?.Invoke(this, e);
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

		protected async Task ProcessPointer(PointerRoutedEventArgs e)
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
					(int row, int column) = await this.GetPixelCoordinates(border);

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

		protected Task<(int Row, int Column)> GetPixelCoordinates(Border border)
		{
			(int Row, int Column) returnValue = (0, 0);

			returnValue.Row = (int)border.GetValue(Grid.RowProperty);
			returnValue.Column = (int)border.GetValue(Grid.ColumnProperty);

			return Task.FromResult(returnValue);
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
