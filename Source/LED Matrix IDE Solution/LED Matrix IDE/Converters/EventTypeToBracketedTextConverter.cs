using System;
using CodeBuilder;
using Windows.UI.Xaml.Data;

namespace LedMatrixIde.Converters
{
	public sealed class EventTypeToBracketedTextConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			string returnValue = String.Empty;

			if (value is BuildEventArgs.BuildEventType eventType)
			{
				return $"[{value.ToString()}]";
			}

			return returnValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotSupportedException();
		}
	}
}
