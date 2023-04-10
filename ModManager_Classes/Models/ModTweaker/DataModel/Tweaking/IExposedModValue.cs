using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.ModTweaker.DataModel.Tweaking
{
    public enum ExposedModValueType { SimpleValue, Enum, Slider, Toggle, SkipToggle }

    public enum ExposedModValueReplaceType { Text, Xml }

    public interface IExposedModValue
    {
        public string Path { get; init; }
        public string ModOpID { get; init; }
        public string ExposeID { get; init; }
        public ExposedModValueType ExposedModValueType { get; init; }
        public ExposedModValueReplaceType ReplaceType { get; init; }
        public TweakerFile Parent { get; init; }

        public string? Description { get; init; }

        public string Value { get; set; }

        public bool IsEnumType { get; }
        public bool IsSimpleValue { get; }
        public bool IsSliderType { get; }
        public bool IsToggleType { get; }
    }
}
