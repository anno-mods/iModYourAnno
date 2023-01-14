using Imya.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Imya.UI.ValueConverters
{
    [ValueConversion(typeof(double), typeof(String))]
    internal sealed class SpeedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double dvalue)
                return Math.Round(dvalue / (1024 * 1024), 2) + " MB/s";
            if (value is long lvalue)
                return Math.Round((float)lvalue / (1024 * 1024), 2) + " MB/s";
            return String.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(long), typeof(String))]
    internal sealed class ByteSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double dvalue)
                return Math.Round(dvalue / (1024 * 1024), 2) + " MB";
            if (value is long lvalue)
                return Math.Round((float)lvalue / (1024 * 1024), 2) + " MB";
            return String.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
