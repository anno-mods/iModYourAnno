﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace Imya.UI.ValueConverters
{
    [ValueConversion(typeof(bool), typeof(String))]
    internal class IconConverter : IValueConverter
    {
        public object Convert(object value, Type TargetType, object parameter, CultureInfo Culture)
        {
            bool b = (bool)value;
            return b ? "CheckBold" : "HighlightOff";
        }

        public object ConvertBack(object value, Type TargetType, object parameter, CultureInfo Culture)
        {
            string strValue = value as string;
            return strValue.Equals("CheckBold");
        }
    }
}
