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
                Expose.TryGetAttribute(TweakerConstants.DESCRIPTION, out String? Description);

                ExposedModValueType type = ExposedModValueType.SimpleValue;
                Expose.TryGetAttribute(TweakerConstants.KIND, out String? Kind);
                if (Kind is String valid_kind && Enum.TryParse<ExposedModValueType>(valid_kind, out var _val))
                {
                    type = _val;
                }

                if (type == ExposedModValueType.Enum)
                {
                    var nodes = Expose.SelectNodes($"./{TweakerConstants.ENUM_HEADER}/{TweakerConstants.ENUM_ENTRY}");
                    if (nodes is not null && nodes.Count > 0)
                    {
                        var predefined_vals = nodes?.Cast<XmlNode>()
                                    .Select(node => node.InnerText)
                                    .ToArray();
                        return new ExposedPredefinedModValue()
                        {
                            Path = Path!,
                            ModOpID = ModOpID!,
                            ExposeID = ExposeID!,
                            Parent = parent,
                            Description = Description,
                            PredefinedValues = predefined_vals
                        };
                    }
                }
                else if (type == ExposedModValueType.Slider)
                {
                    var node = Expose.SelectSingleNode("./SliderDefinition");
                    if (node is XmlNode slider_definition

                        && slider_definition.TryGetAttribute("Min", out var MinStr)
                        && slider_definition.TryGetAttribute("Max", out var MaxStr)
                        && slider_definition.TryGetAttribute("Stepping", out var SteppingStr)

                        && float.TryParse(MinStr, out float min)
                        && float.TryParse(MaxStr, out float max)
                        && float.TryParse(SteppingStr, out float stepping))
                    {
                        return new ExposedRangedModValue()
                        {
                            Path = Path!,
                            ModOpID = ModOpID!,
                            ExposeID = ExposeID!,
                            Parent = parent,
                            Description = Description,
                            Min = min,
                            Max = max,
                            Stepping = stepping
                        };
                    }
                }
                else if (type == ExposedModValueType.Toggle)
                {
                    var falseval = Expose.SelectSingleNode($"./{TweakerConstants.ALT_TOGGLE_VAL}");

                    Expose.TryGetAttribute(TweakerConstants.INVERTED, out var invert);

                    bool IsInverted = invert?.Equals("True") ?? false;

                    if (falseval is XmlNode valid_falseval)
                    {
                        var val = new ExposedToggleModValue()
                        {
                            Path = Path!,
                            ModOpID = ModOpID!,
                            ExposeID = ExposeID!,
                            Parent = parent,
                            Description = Description,
                            FalseValue = valid_falseval.InnerXml,
                            IsInverted = IsInverted
                        };
                        val.InitTrueValue();
                        return val; 
                    }
                }
                return new ExposedModValue()
                {
                    Path = Path!,
                    ModOpID = ModOpID!,
                    ExposeID = ExposeID!,
                    Parent = parent
                };
            }
            return null;
        }
    }
}
