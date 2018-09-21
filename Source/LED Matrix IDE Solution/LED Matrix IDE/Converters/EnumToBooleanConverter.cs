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
using LedMatrixIde.Helpers;
using Windows.UI.Xaml.Data;

namespace LedMatrixIde.Converters
{
	public class EnumToBooleanConverter : IValueConverter
	{
		public Type EnumType { get; set; }

		public object Convert(object value, Type targetType, object parameter, string language)
		{
			bool returnValue = false;

			if (parameter is string enumString)
			{
				if (Enum.IsDefined(this.EnumType, value))
				{
					object enumValue = Enum.Parse(this.EnumType, enumString);

					returnValue = enumValue.Equals(value);
				}
				else
				{
					throw new ArgumentException("ExceptionEnumToBooleanConverterValueMustBeAnEnum".GetLocalized());
				}
			}
			else
			{
				throw new ArgumentException("ExceptionEnumToBooleanConverterParameterMustBeAnEnumName".GetLocalized());
			}

			return returnValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			object returnValue = null;

			if (parameter is string enumString)
			{
				returnValue = Enum.Parse(this.EnumType, enumString);
			}
			else
			{
				throw new ArgumentException("ExceptionEnumToBooleanConverterParameterMustBeAnEnumName".GetLocalized());
			}

			return returnValue;
		}
	}
}
