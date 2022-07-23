using Imya.Models.NotifyPropertyChanged;
using Imya.Utils;
using System.Text.RegularExpressions;
using System.Xml;

namespace Imya.Models.ModTweaker
{
    public enum ExposedModValueType { SimpleValue, Enum }
    public class ExposedModValue
    {
        public String Path;
        public String ModOpID;
        public String ExposeID { get; private set; }

        public String[]? PredefinedValues { get; private set; }

        public String Value
        {
            get => _value;
            set
            {
                _value = value;
                Parent.TweakStorage.SetTweakValue(Parent.FilePath, ExposeID, Value);
            }
        }

        public ExposedModValueType ExposedModValueType;

        public ExposedModValue()
        {
            ExposedModValueType = ExposedModValueType.SimpleValue;
        }

        public ExposedModValue(ExposedModValueType val)
        {
            ExposedModValueType = val;
        }

        public TweakerFile Parent { get; set; }

        private String _value;

        public bool IsEnumType { get => ExposedModValueType == ExposedModValueType.Enum; }

        public bool IsSimpleValue { get => ExposedModValueType == ExposedModValueType.SimpleValue; }

        public static ExposedModValue? FromXmlNode(XmlNode Expose, TweakerFile parent)
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

                var value = new ExposedModValue(type) { Path = Path!, ModOpID = ModOpID!, ExposeID = ExposeID!, Parent = parent };

                if (type == ExposedModValueType.Enum)
                {
                    var nodes = Expose.SelectNodes("./FixedValues/Value");
                    value.PredefinedValues = nodes?.Cast<XmlNode>()
                               .Select(node => node.InnerText)
                               .ToArray();
                }
                return value;
            }
            return null;
        }
    }

}
