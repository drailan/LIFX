using System;
using System.Globalization;
using System.Windows.Data;

namespace LIFXGui.Converters
{
	class ButtonUIntToStateConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (ushort)value != 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
