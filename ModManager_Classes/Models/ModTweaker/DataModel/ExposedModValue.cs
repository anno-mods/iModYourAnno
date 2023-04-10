using Imya.Models.NotifyPropertyChanged;
using Imya.Utils;
using System.Text.RegularExpressions;
using System.Xml;

namespace Imya.Models.ModTweaker.DataModel
{
    public class ExposedModValue : PropertyChangedNotifier, IExposedModValue
    {
        public string Path { get; init; }
        public string ModOpID { get; init; }
        public string ExposeID { get; init; }
        public string? Description { get; init; }
        public string? Tooltip { get; init; }

        public ExposedModValueType ExposedModValueType { get; init; }
        public ExposedModValueReplaceType ReplaceType { get; init; }
        public TweakerFile Parent { get; init; }

        public string Value
        {
            get => _value;
            set
            {
                SetProperty(ref _value, value);
                Parent.TweakStorage.SetTweakValue(Parent.FilePath, ExposeID, Value);
            }
        }
        private string _value;

        public ExposedModValue()
        {
            ExposedModValueType = ExposedModValueType.SimpleValue;
            ReplaceType = ExposedModValueReplaceType.Text;
        }

        public bool IsEnumType { get => ExposedModValueType == ExposedModValueType.Enum; }
        public bool IsSimpleValue { get => ExposedModValueType == ExposedModValueType.SimpleValue; }
        public bool IsSliderType { get => ExposedModValueType == ExposedModValueType.Slider; }
        public bool IsToggleType { get => ExposedModValueType == ExposedModValueType.Toggle || ExposedModValueType == ExposedModValueType.SkipToggle; }

    }

}
