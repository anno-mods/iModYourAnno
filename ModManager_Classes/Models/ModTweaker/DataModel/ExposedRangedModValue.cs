using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.ModTweaker.DataModel
{
    public class ExposedRangedModValue : ExposedModValue
    {
        public float Min { get; init; }
        public float Max { get; init; }
        public float Stepping { get; init; }

        public ExposedRangedModValue() : base()
        {
            ExposedModValueType = ExposedModValueType.Slider;
        }
    }

}
