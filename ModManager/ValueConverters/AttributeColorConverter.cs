using Imya.Models;
using Imya.Models.Attributes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Imya.UI.ValueConverters
{
    [ValueConversion(typeof(IAttribute), typeof(SolidColorBrush))]
    public class AttributeColorConverter : IValueConverter
    {
        private SolidColorBrush Fallback = new SolidColorBrush(Colors.Black);

        public object Convert(object value, Type TargetType, object parameter, CultureInfo Culture)
        {
            if (value is not IAttribute attrib) return String.Empty;

            return attrib.AttributeType switch
            {
                AttributeType.ModStatus => ConvertModStatus((attrib as ModStatusAttribute)?.Status),
                AttributeType.ModCompabilityIssue => FindResourceBrush("ErrorColorBrush"),
                AttributeType.UnresolvedDependencyIssue => FindResourceBrush("ErrorColorBrush"),
                AttributeType.TweakedMod => FindResourceBrush("InformationColorBrush"),
                AttributeType.MissingModinfo => FindResourceBrush("InformationColorBrush"),
                AttributeType.ModContentInSubfolder => FindResourceBrush("ErrorColorBrush"),
                _ => FindResourceBrush("TextColorBrush"),
            };
        }

        private SolidColorBrush ConvertModStatus(ModStatus? status)
        {
            if (status is not ModStatus valid_status) return Fallback;
            switch (valid_status)
            {
                case ModStatus.Updated: return FindResourceBrush("HighlightColorBrush");
                case ModStatus.New: return FindResourceBrush("HighlightColorBrush");
                case ModStatus.Obsolete: return FindResourceBrush("ErrorColorBrush");
            }
            return Fallback;
        }

        private SolidColorBrush FindResourceBrush(String ResourceName)
        {
            return Application.Current.Resources[ResourceName] as SolidColorBrush ?? Fallback;
        }

        public object ConvertBack(object value, Type TargetType, object parameter, CultureInfo Culture)
        {
            throw new NotImplementedException();
        }
    }
}
