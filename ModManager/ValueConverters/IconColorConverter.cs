using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;
using System.Windows.Media;

namespace Imya.UI.ValueConverters
{
    [ValueConversion(typeof(bool), typeof(String))]
    internal class IconColorConverter : IValueConverter
    {
        public object Convert(object value, Type TargetType, object parameter, CultureInfo Culture)
        {
            bool b = (bool)value;

            try
            {
                SolidColorBrush? ColorActive = Application.Current.Resources["ModActiveColorBrush"] as SolidColorBrush;
                SolidColorBrush? ColorInactive = Application.Current.Resources["ModInactiveColorBrush"] as SolidColorBrush;

                if (ColorActive is not null 
                    && ColorInactive is not null)
                { 
                    return b ? ColorActive : ColorInactive;
                }
            }
            catch(Exception ex)
            {

            }

            //fallback
            return b ? "Green" : "Gray";
        }

        public object ConvertBack(object value, Type TargetType, object parameter, CultureInfo Culture)
        {
            throw new NotImplementedException("We don't need to convert from colors :)");
        }
    }
}
