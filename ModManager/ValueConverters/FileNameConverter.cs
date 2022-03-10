using Imya.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Imya.UI.ValueConverters
{
    [ValueConversion(typeof(String), typeof(String))]
    public class FileNameConverter : IValueConverter
    {
        public object Convert(object value, Type TargetType, object parameter, CultureInfo Culture)
        {
            String s = value as String;
            if (s is not null)
            {
                return Path.GetFileName(s);
            }
            return String.Empty;
        }

        public object ConvertBack(object value, Type TargetType, object parameter, CultureInfo Culture)
        {
            throw new NotImplementedException();
        }
    }
}
