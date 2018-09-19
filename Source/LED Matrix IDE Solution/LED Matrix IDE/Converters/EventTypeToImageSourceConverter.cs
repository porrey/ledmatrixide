using System;
using CodeBuilder;
using Windows.UI.Xaml.Data;

namespace LedMatrixIde.Converters
{
	public sealed class EventTypeToImageSourceConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			string returnValue = String.Empty;

			if (value is BuildEventArgs.BuildEventType eventType)
			{
				returnValue = $"/Assets/{value.ToString().ToLower()}.png";
			}

			return returnValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotSupportedException();
		}
	}
}
