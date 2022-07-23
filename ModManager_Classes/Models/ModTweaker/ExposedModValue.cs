using Imya.Models.NotifyPropertyChanged;
using Imya.Utils;
using System.Text.RegularExpressions;
using System.Xml;

namespace Imya.Models.ModTweaker
{
    public class ExposedModValue : IExposedModValue
    {
        public String Path { get; init; }
        public String ModOpID { get; init; }
        public String ExposeID { get; init; }
        public ExposedModValueType ExposedModValueType { get; init; }
        public TweakerFile Parent { get; init; }

        public String Value
        {
            get => _value;
            set
            {
                _value = value;
                Parent.TweakStorage.SetTweakValue(Parent.FilePath, ExposeID, Value);
            }
        }
        private String _value;

        public ExposedModValue()
        {
            ExposedModValueType = ExposedModValueType.SimpleValue;
        }
    }

}
