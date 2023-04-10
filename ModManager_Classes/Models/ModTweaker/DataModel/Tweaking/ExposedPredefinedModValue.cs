using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.ModTweaker.DataModel.Tweaking
{
    public class ExposedPredefinedModValue : ExposedModValue
    {
        public string[]? PredefinedValues { get; init; }

        public ExposedPredefinedModValue() : base()
        {
            ExposedModValueType = ExposedModValueType.Enum;
        }
    }
}
