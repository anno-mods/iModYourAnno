using Imya.Models.Attributes;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Imya.UI.ValueConverters
{
    static class AttributeIcons
    {
        public static (string, SolidColorBrush) AttributeToIcon(AttributeType type, ModStatus? status)
        {
            return type switch
            {
                AttributeType.ModStatus when status == ModStatus.Updated => ("Update", FindResourceBrush("HighlightColorBrush")),
                AttributeType.ModStatus when status == ModStatus.New => ("Download", FindResourceBrush("HighlightColorBrush")),
                AttributeType.ModStatus when status == ModStatus.Obsolete => ("RemoveCircleOutline", FindResourceBrush("ErrorColorBrush")),
                AttributeType.ModCompabilityIssue => ("AlertBox", FindResourceBrush("ErrorColorBrush")),
                AttributeType.UnresolvedDependencyIssue => ("FileTree", FindResourceBrush("ErrorColorBrush")),
                AttributeType.TweakedMod => ("Tools", FindResourceBrush("InformationColorBrush")),
                AttributeType.MissingModinfo => ("HelpBox", FindResourceBrush("InformationColorBrush")),
                AttributeType.ModContentInSubfolder => ("AlertBox", FindResourceBrush("ErrorColorBrush")),
                AttributeType.IssueModRemoved => ("RemoveCircleOutline", FindResourceBrush("ErrorColorBrush")),
                AttributeType.IssueModAccess => ("RemoveCircleOutline", FindResourceBrush("ErrorColorBrush")),
                _ => ("InformationOutline", FindResourceBrush("TextColorBrush")),
            };
        }

        private static SolidColorBrush FindResourceBrush(String ResourceName)
        {
            return Application.Current.Resources[ResourceName] as SolidColorBrush ?? Fallback;
        }

        private static readonly SolidColorBrush Fallback = new(Colors.Black);
    }

    [ValueConversion(typeof(IAttribute), typeof(SolidColorBrush))]
    public class AttributeColorConverter : IValueConverter
    {
        public object Convert(object value, Type TargetType, object parameter, CultureInfo Culture)
        {
            if (value is not IAttribute attrib) 
                return string.Empty;

            return AttributeIcons.AttributeToIcon(attrib.AttributeType, (attrib as ModStatusAttribute)?.Status).Item2;
        }

        public object ConvertBack(object value, Type TargetType, object parameter, CultureInfo Culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(IAttribute), typeof(string))]
    public class AttributeIconConverter : IValueConverter
    {
        public object Convert(object value, Type TargetType, object parameter, CultureInfo Culture)
        {
            if (value is not IAttribute attrib) return
                string.Empty;

            return AttributeIcons.AttributeToIcon(attrib.AttributeType, (attrib as ModStatusAttribute)?.Status).Item1;
        }

        public object ConvertBack(object value, Type TargetType, object parameter, CultureInfo Culture)
        {
            throw new NotImplementedException();
        }
    }
}
