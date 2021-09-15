using System;
using System.Globalization;
using Xamarin.Forms;

namespace SSICPAS.Controls.Converters
{
    public class BoolToInverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(bool))
            {
                return !(bool)value;
            }

            if (targetType == typeof(bool?))
            {
                return !(bool)value;
            }

            throw new InvalidOperationException("Can't convert non-boolean");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(bool))
            {
                throw new InvalidOperationException("Can't convert non-boolean");
            }

            return !(bool) value;
        }
    }
}