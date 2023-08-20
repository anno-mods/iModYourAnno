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
    
    [ValueConversion(typeof(Uri), typeof(Uri))]
    public class FallbackImageConverter : IValueConverter
    {
        static Uri fallback = new Uri(Path.Combine(Environment.CurrentDirectory, "resources", "modbanner_placeholder.png"));

        static readonly string[] filenames = {
            "banner.png",
            "banner.jpg"
        };

        public object Convert(object value, Type TargetType, object parameter, CultureInfo Culture)
        {
            var str = value as Uri;
            if (str is null)
                return fallback.AbsoluteUri;
            if(str.IsFile && !File.Exists(str.LocalPath))
                return fallback.AbsoluteUri;
            return str.AbsoluteUri;
        }

        public object ConvertBack(object value, Type TargetType, object parameter, CultureInfo Culture)
        {
            throw new NotImplementedException();
        }
    }
}
