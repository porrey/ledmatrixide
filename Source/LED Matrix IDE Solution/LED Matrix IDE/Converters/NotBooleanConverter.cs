using System;

using Windows.UI.Xaml.Data;

namespace LedMatrixIde.Converters
{
    public class NotBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
			object returnValue = false;

			if (value is bool b)
			{
				returnValue = !b;
			}

			return returnValue;
		}

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
			object returnValue = false;

			if (value is bool b)
			{
				returnValue = !b;
			}

			return returnValue;
		}
    }
}
