using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace LedMatrixIde.Converters
{
	public sealed class BooleanToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language) =>
			(value is bool && (bool)value) ? Visibility.Visible : Visibility.Collapsed;

		public object ConvertBack(object value, Type targetType, object parameter, string language) =>
			value is Visibility && (Visibility)value == Visibility.Visible;
	}
}
