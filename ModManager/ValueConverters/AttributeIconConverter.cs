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
            if (value is not IAttribute attrib) return String.Empty;

            switch (attrib.AttributeType)
            {
                case AttributeType.ModStatus: return ConvertModStatus((attrib as ModStatusAttribute)?.Status);
                case AttributeType.ModCompabilityIssue: return "AlertBox";
                case AttributeType.UnresolvedDependencyIssue: return "FileTree";
                case AttributeType.TweakedMod: return "Tools";
                case AttributeType.MissingModinfo: return "HelpBox";
            }
            return String.Empty;
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
