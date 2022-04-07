using Imya.Models;
using Imya.UI.Popup;
using Imya.Utils;
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
    [ValueConversion(typeof(FilenameValidation), typeof(LocalizedText))]
    public class FilenameValidationConverter : IValueConverter
    {
        public object Convert(object value, Type TargetType, object parameter, CultureInfo Culture)
        {
            var status = (FilenameValidation)value;

            switch (status)
            {
                case FilenameValidation.Invalid: return TextManager.Instance["PROFILESAVE_INVALID"];
                case FilenameValidation.AlreadyExists: return TextManager.Instance["PROFILESAVE_ALREADYEXISTS"];
            }

            return IText.Empty;
        }

        public object ConvertBack(object value, Type TargetType, object parameter, CultureInfo Culture)
        {
            throw new NotImplementedException();
        }
    }
}
