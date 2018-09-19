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
