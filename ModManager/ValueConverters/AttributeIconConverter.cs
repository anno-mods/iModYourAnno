using Imya.Models;
using Imya.Models.Attributes;
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
    [ValueConversion(typeof(IAttribute), typeof(String))]
    public class AttributeIconConverter : IValueConverter
    {
        public object Convert(object value, Type TargetType, object parameter, CultureInfo Culture)
        {
            if (value is not IAttribute attrib) return
                string.Empty;

            return attrib.AttributeType switch
            {
                AttributeType.ModStatus => ConvertModStatus((attrib as ModStatusAttribute)?.Status),
                AttributeType.ModCompabilityIssue => "AlertBox",
                AttributeType.UnresolvedDependencyIssue => "FileTree",
                AttributeType.TweakedMod => "Tools",
                AttributeType.MissingModinfo => "HelpBox",
                AttributeType.ModContentInSubfolder => "AlertBox",
                _ => "InformationOutline",
            };
        }

        private String ConvertModStatus(ModStatus? status)
        {
            if (status is not ModStatus valid_status) return String.Empty;
            switch (valid_status)
            {
                case ModStatus.Updated: return "Update";
                case ModStatus.New: return "Download";
                case ModStatus.Obsolete: return "RemoveCircleOutline";
            }
            return String.Empty;
        }

        public object ConvertBack(object value, Type TargetType, object parameter, CultureInfo Culture)
        {
            throw new NotImplementedException();
        }
    }
}
