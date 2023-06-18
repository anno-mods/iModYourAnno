using Imya.Models;
using Imya.Texts;
using Imya.UI.Popup;
using Imya.Utils;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace Imya.UI.ValueConverters
{
    [ValueConversion(typeof(FilenameValidation), typeof(LocalizedText))]
    public class FilenameValidationConverter : IValueConverter
    {
        private readonly ITextManager _textManager;
        public FilenameValidationConverter(ITextManager textManager)
        {
            _textManager = textManager;
        }

        public object Convert(object value, Type TargetType, object parameter, CultureInfo Culture)
        {
            var status = (FilenameValidation)value;

            switch (status)
            {
                case FilenameValidation.Invalid: return _textManager["PROFILESAVE_INVALID"];
                case FilenameValidation.AlreadyExists: return _textManager["PROFILESAVE_ALREADYEXISTS"];
            }

            return IText.Empty;
        }

        public object ConvertBack(object value, Type TargetType, object parameter, CultureInfo Culture)
        {
            throw new NotImplementedException();
        }
    }
}
