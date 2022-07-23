using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.ModTweaker
{
    public enum ExposedModValueType { SimpleValue, Enum }

    public interface IExposedModValue
    {
        public String Path { get; init; }
        public String ModOpID { get; init; }
        public String ExposeID { get; init; }
        public ExposedModValueType ExposedModValueType { get; init; }
        public TweakerFile Parent { get; init; }

        public String Value { get; set; }

        public bool IsEnumType { get => ExposedModValueType == ExposedModValueType.Enum; }
        public bool IsSimpleValue { get => ExposedModValueType == ExposedModValueType.SimpleValue; }
    }

}
