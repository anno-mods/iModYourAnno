using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.ModTweaker
{
    public class ExposedPredefinedModValue : ExposedModValue
    {
        public String[]? PredefinedValues { get; init; }

        public ExposedPredefinedModValue() : base() {
            ExposedModValueType = ExposedModValueType.Enum;
        }
    }
}
