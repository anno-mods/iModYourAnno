using Imya.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Imya.Models.ModTweaker
{
    public class ExposedModValueFactory
    {

        public static IExposedModValue? FromXmlNode(XmlNode Expose, TweakerFile parent)
        {
            if (Expose.TryGetAttribute(TweakerConstants.EXPOSE_PATH, out String? Path)
                && Expose.TryGetAttribute(TweakerConstants.MODOP_ID, out String? ModOpID)
                && Expose.TryGetAttribute(TweakerConstants.EXPOSE_ATTR, out String? ExposeID))
            {
                ExposedModValueType type = ExposedModValueType.SimpleValue;
                Expose.TryGetAttribute(TweakerConstants.KIND, out String? Kind);
                if (Kind is String valid_kind && Enum.TryParse<ExposedModValueType>(valid_kind, out var _val))
                {
                    type = _val;
                }

                if (type == ExposedModValueType.Enum)
                {
                    var nodes = Expose.SelectNodes("./FixedValues/Value");

                    var predefined_vals = nodes?.Cast<XmlNode>()
                                .Select(node => node.InnerText)
                                .ToArray();
                    return new ExposedPredefinedModValue()
                    {
                        Path = Path!,
                        ModOpID = ModOpID!,
                        ExposeID = ExposeID!,
                        Parent = parent,
                        PredefinedValues = predefined_vals
                    };

                }
                else
                { 
                    return new ExposedModValue()
                    {
                        Path = Path!,
                        ModOpID = ModOpID!,
                        ExposeID = ExposeID!,
                        Parent = parent
                    };
                }
            }
            return null;
        }
    }
}
