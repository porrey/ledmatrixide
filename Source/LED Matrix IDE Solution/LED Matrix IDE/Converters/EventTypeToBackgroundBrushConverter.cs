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
