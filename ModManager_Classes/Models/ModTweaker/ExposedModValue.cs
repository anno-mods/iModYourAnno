using Imya.Models.NotifyPropertyChanged;
using Imya.Utils;
using System.Text.RegularExpressions;
using System.Xml;

namespace Imya.Models.ModTweaker
{
    public class ExposedModValue
    {
        public String Path;
        public String ModOpID;
        public String ExposeID { get; private set; }
        public String Value
        {
            get => _value;
            set
            {
                _value = value;
                Parent.TweakStorage.SetTweakValue(Parent.FilePath, ExposeID, Value);
            }
        }

        public TweakerFile Parent { get; set; }

        private String _value;

        public static ExposedModValue? FromXmlNode(XmlNode Expose, TweakerFile parent)
        {
            if (Expose.TryGetAttribute(TweakerConstants.EXPOSE_PATH, out String? Path)
                && Expose.TryGetAttribute(TweakerConstants.MODOP_ID, out String? ModOpID)
                && Expose.TryGetAttribute(TweakerConstants.EXPOSE_ATTR, out String? ExposeID))
            {
                return new ExposedModValue() { Path = Path!, ModOpID = ModOpID!, ExposeID = ExposeID!, Parent = parent};
            }
            return null;
        }
    }

}
