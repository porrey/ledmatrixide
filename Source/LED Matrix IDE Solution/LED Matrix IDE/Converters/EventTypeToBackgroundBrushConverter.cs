using System;
using CodeBuilder;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace LedMatrixIde.Converters
{
	public sealed class EventTypeToBackgroundBrushConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			SolidColorBrush returnValue = null;

			if (value is BuildEventArgs.BuildEventType eventType)
			{
				switch (eventType)
				{
					case BuildEventArgs.BuildEventType.Information:
						returnValue = new SolidColorBrush(Color.FromArgb(255, 107, 170, 202));
						break;
					case BuildEventArgs.BuildEventType.Warning:
						returnValue = new SolidColorBrush(Color.FromArgb(255, 240, 239, 125));
						break;
					case BuildEventArgs.BuildEventType.Error:
						returnValue = new SolidColorBrush(Color.FromArgb(255, 255, 133, 133));
						break;
				}
			}

			return returnValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotSupportedException();
		}
	}
}
