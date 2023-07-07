using Imya.Enums;
using Imya.Texts;
using Imya.UI.ValueConverters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.Attributes.Factories
{
    public class DlcOwnershipAttributeFactory
    {
        private ITextManager _textManager;
        private DlcTextConverter _textConverter; 

        public DlcOwnershipAttributeFactory(
            ITextManager textManager,
            DlcTextConverter textConverter) 
        {
            _textManager = textManager;
            _textConverter = textConverter;
        }

        public IAttribute Get(IEnumerable<DlcId> missing)
        {
            var textTemplate = _textManager.GetText("ATTRIBUTE_MISSINGDLC");
            var texts = missing.Select(x => (_textConverter.Convert(x, typeof(IText), null, CultureInfo.CurrentCulture) as IText)?.Text );
            return new GenericAttribute()
            {
                AttributeType = AttributeType.DlcNotOwned,
                Description = new SimpleText(String.Format(textTemplate.Text, String.Join(",\n- ", texts)))
            };
        }
    }
}
