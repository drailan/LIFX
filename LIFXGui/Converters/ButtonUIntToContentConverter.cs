using System;
using System.Globalization;
using System.Windows.Data;

namespace LIFXGui.Converters
{
	class ButtonUIntToContentConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (ushort)value == 0 ? "Off" : "On";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
