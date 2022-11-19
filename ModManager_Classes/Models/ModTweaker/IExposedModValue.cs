using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.ModTweaker
{
    public enum ExposedModValueType { SimpleValue, Enum, Slider, Toggle, SkipToggle }

    public enum ExposedModValueReplaceType { Text, Xml }

    public interface IExposedModValue
    {
        public String Path { get; init; }
        public String ModOpID { get; init; }
        public String ExposeID { get; init; }
        public ExposedModValueType ExposedModValueType { get; init; }
        public ExposedModValueReplaceType ReplaceType { get; init;}
        public TweakerFile Parent { get; init; }

        public String? Description { get; init; }

        public String Value { get; set; }

        public bool IsEnumType { get; }
        public bool IsSimpleValue { get; }
        public bool IsSliderType { get; }
        public bool IsToggleType { get; }
    }
}
