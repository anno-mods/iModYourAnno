using Imya.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace Imya.Models.ModTweaker.DataModel.Tweaking
{
    public class ExposedModValueFactory
    {
        public static IExposedModValue? FromXmlNode(XmlNode Expose, TweakerFile parent)
        {
            if (Expose.TryGetAttribute(TweakerConstants.EXPOSE_PATH, out string? Path)
                && Expose.TryGetAttribute(TweakerConstants.MODOP_ID, out string? ModOpID)
                && Expose.TryGetAttribute(TweakerConstants.EXPOSE_ATTR, out string? ExposeID))
            {
                Expose.TryGetAttribute(TweakerConstants.DESCRIPTION, out string? description);
                Expose.TryGetAttribute(TweakerConstants.TOOLTIP, out string? tooltip);

                try
                {
                    var checkPath = XPathExpression.Compile(Path);
                }
                catch (XPathException e)
                {
                    Console.WriteLine($"Invalid XPath: {ExposeID} - {Path}");
                    return null; 
                }

                ExposedModValueType type = ExposedModValueType.SimpleValue;
                Expose.TryGetAttribute(TweakerConstants.KIND, out string? Kind);
                if (Kind is string valid_kind && Enum.TryParse<ExposedModValueType>(valid_kind, out var _val))
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
                            Description = description,
                            Tooltip = tooltip,
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
                            Description = description,
                            Tooltip = tooltip,
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
                            Description = description,
                            Tooltip = tooltip,
                            FalseValue = valid_falseval.InnerXml,
                            IsInverted = IsInverted
                        };
                        val.InitTrueValue();
                        return val;
                    }
                }
                else if (type == ExposedModValueType.SkipToggle)
                {
                    Expose.TryGetAttribute(TweakerConstants.INVERTED, out var invert);
                    bool IsInverted = invert?.Equals("True") ?? false;

                    var val = new ExposedToggleModValue()
                    {
                        Path = Path!,
                        ModOpID = ModOpID!,
                        ExposeID = ExposeID!,
                        Parent = parent,
                        Description = description,
                        Tooltip = tooltip,
                        FalseValue = "1",
                        TrueValue = "0",
                        IsInverted = IsInverted,
                        ExposedModValueType = ExposedModValueType.SkipToggle
                    };
                    return val;
                }
                return new ExposedModValue()
                {
                    Path = Path!,
                    ModOpID = ModOpID!,
                    ExposeID = ExposeID!,
                    Parent = parent,
                    Description = description
                };
            }
            return null;
        }
    }
}
