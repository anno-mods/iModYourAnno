using Anno.EasyMod.Attributes;
using Imya.Models.Attributes;
using Imya.Models.Attributes.Factories;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Imya.UI.ValueConverters
{
    static class AttributeIcons
    {
        public static (string, SolidColorBrush) AttributeToIcon(string type, ModStatus? status)
        {
            return type switch
            {
                AttributeTypes.ModStatus when status == ModStatus.Updated => ("Update", FindResourceBrush("HighlightColorBrush")),
                AttributeTypes.ModStatus when status == ModStatus.New => ("Download", FindResourceBrush("HighlightColorBrush")),
                AttributeTypes.ModStatus when status == ModStatus.Obsolete => ("RemoveCircleOutline", FindResourceBrush("ErrorColorBrush")),
                AttributeTypes.ModCompabilityIssue => ("AlertBox", FindResourceBrush("ErrorColorBrush")),
                AttributeTypes.ModReplacedByIssue => ("RemoveCircleOutline", FindResourceBrush("ErrorColorBrush")),
                AttributeTypes.UnresolvedDependencyIssue => ("FileTree", FindResourceBrush("ErrorColorBrush")),
                AttributeTypes.TweakedMod => ("Tools", FindResourceBrush("InformationColorBrush")),
                AttributeTypes.MissingModinfo => ("HelpBox", FindResourceBrush("InformationColorBrush")),
                AttributeTypes.ModContentInSubfolder => ("AlertBox", FindResourceBrush("ErrorColorBrush")),
                AttributeTypes.IssueModRemoved => ("TrashCanOutline", FindResourceBrush("ErrorColorBrush")),
                AttributeTypes.IssueModAccess => ("FolderAlertOutline", FindResourceBrush("ErrorColorBrush")),
                AttributeTypes.CyclicDependency => ("CircleArrows", FindResourceBrush("ErrorColorBrush")),
                AttributeTypes.DlcNotOwned => ("Ubisoft", FindResourceBrush("InformationColorBrush")),
                _ => ("InformationOutline", FindResourceBrush("TextColorBrush")),
            };
        }

        private static SolidColorBrush FindResourceBrush(String ResourceName)
        {
            return Application.Current.Resources[ResourceName] as SolidColorBrush ?? Fallback;
        }

        private static readonly SolidColorBrush Fallback = new(Colors.Black);
    }

    [ValueConversion(typeof(IModAttribute), typeof(SolidColorBrush))]
    public class AttributeColorConverter : IValueConverter
    {
        public object Convert(object value, Type TargetType, object parameter, CultureInfo Culture)
        {
            if (value is not IModAttribute attrib) 
                return string.Empty;

            return AttributeIcons.AttributeToIcon(attrib.AttributeType, (attrib as ModStatusAttribute)?.Status).Item2;
        }

        public object ConvertBack(object value, Type TargetType, object parameter, CultureInfo Culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(IModAttribute), typeof(string))]
    public class AttributeIconConverter : IValueConverter
    {
        public object Convert(object value, Type TargetType, object parameter, CultureInfo Culture)
        {
            if (value is not IModAttribute attrib) return
                string.Empty;

            return AttributeIcons.AttributeToIcon(attrib.AttributeType, (attrib as ModStatusAttribute)?.Status).Item1;
        }

        public object ConvertBack(object value, Type TargetType, object parameter, CultureInfo Culture)
        {
            throw new NotImplementedException();
        }
    }
}
