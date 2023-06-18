using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Imya.Models.ModTweaker.DataModel.Tweaking
{
    public class ExposedToggleModValue : ExposedModValue
    {
        public string TrueValue { get; set; }
        public string FalseValue { get; set; }

        public bool IsTrue
        {
            get => _isTrue;
            set
            {
                SetProperty(ref _isTrue, value);
                Value = _isTrue ?
                    IsInverted ? FalseValue : TrueValue :
                    IsInverted ? TrueValue : FalseValue;
            }
        }
        private bool _isTrue;

        public bool IsInverted { get; init; } = true;

        public ExposedToggleModValue() : base()
        {
            ExposedModValueType = ExposedModValueType.Toggle;
            ReplaceType = ExposedModValueReplaceType.Xml;
        }

        public void InitTrueValue()
        {
            Task.Run(() =>
            {
                var vals = Parent?.ModOps
                .Where(x => x.HasID && x.ID!.Equals(ModOpID))
                .FirstOrDefault()?
                .Code
                .Cast<XmlNode>()
                .Select(x => x.SelectSingleNode(Path))
                .FirstOrDefault()?
                .OuterXml;
                TrueValue = vals is not null ? string.Join("", vals) : string.Empty;
            });

        }
    }
}
