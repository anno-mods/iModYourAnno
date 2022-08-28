using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Imya.UI.ValueConverters
{
    [ValueConversion(typeof(string), typeof(bool))]
    internal sealed class IsEmptyToBool : IValueConverter
    {
        public bool OnEmpty { get; set; }

        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value is not string str)
                return OnEmpty;
            return string.IsNullOrWhiteSpace(str) ? OnEmpty : !OnEmpty;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            // be silent
            return string.Empty;
        }
    }

    [ValueConversion(typeof(string), typeof(Visibility))]
    internal sealed class IsEmptyToVisibility : IValueConverter
    {
        public Visibility OnEmpty { get; set; }
        public Visibility OnElse { get; set; }

        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value is not string str)
                return OnEmpty;
            return string.IsNullOrWhiteSpace(str) ? OnEmpty : OnElse;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            // be silent
            return string.Empty;
        }
    }
}
