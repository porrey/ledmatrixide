using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace LedMatrixIde.Converters
{
	public class ColorToSolidColorBrushConverter : IValueConverter
    {
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			SolidColorBrush returnValue = null;

			if (value is Color color)
			{
				returnValue = new SolidColorBrush(color);
			}

			return returnValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotSupportedException();
		}
	}
}
