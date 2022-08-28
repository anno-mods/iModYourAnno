using Imya.Models.NotifyPropertyChanged;
using Imya.Utils;
using System.Text.RegularExpressions;
using System.Xml;

namespace Imya.Models.ModTweaker
{
    public class ExposedModValue : PropertyChangedNotifier, IExposedModValue
    {
        public String Path { get; init; }
        public String ModOpID { get; init; }
        public String ExposeID { get; init; }
        public String? Description { get; init; }
        public String? Tooltip { get; init; }

        public ExposedModValueType ExposedModValueType { get; init; }
        public ExposedModValueReplaceType ReplaceType { get; init; }
        public TweakerFile Parent { get; init; }

        public String Value
        {
            get => _value;
            set
            {
                SetProperty(ref _value, value);
                Parent.TweakStorage.SetTweakValue(Parent.FilePath, ExposeID, Value);
            }
        }
        private String _value;

        public ExposedModValue()
        {
            ExposedModValueType = ExposedModValueType.SimpleValue;
            ReplaceType = ExposedModValueReplaceType.Text;
        }

        public bool IsEnumType { get => ExposedModValueType == ExposedModValueType.Enum; }
        public bool IsSimpleValue { get => ExposedModValueType == ExposedModValueType.SimpleValue; }
        public bool IsSliderType { get => ExposedModValueType == ExposedModValueType.Slider; }
        public bool IsToggleType { get => ExposedModValueType == ExposedModValueType.Toggle; }
       
    }

}
