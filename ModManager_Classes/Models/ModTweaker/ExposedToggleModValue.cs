using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.ModTweaker
{
    public class ExposedToggleModValue : ExposedModValue
    {
        public String TrueValue { get; init; }
        public String FalseValue { get; init; }

        public bool IsTrue { 
            get => _isTrue;
            set {
                SetProperty(ref _isTrue, value);
                Value = _isTrue ? TrueValue : FalseValue;
            }
        }
        private bool _isTrue;

        public ExposedToggleModValue() : base() {
            ExposedModValueType = ExposedModValueType.Toggle;
            ReplaceType = ExposedModValueReplaceType.Xml;
        }
    }
}
