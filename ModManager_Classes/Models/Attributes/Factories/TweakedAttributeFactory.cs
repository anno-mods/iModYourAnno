using Imya.Models.Attributes.Interfaces;
using Imya.Texts;
using Imya.Utils;

namespace Imya.Models.Attributes.Factories
{
    public class TweakedAttributeFactory : ITweakedAttributeFactory
    {
        private readonly ITextManager _textManager;
        private GenericAttribute TweakedAttribute;

        public TweakedAttributeFactory(ITextManager textManager)
        {
            _textManager = textManager;

            TweakedAttribute = new GenericAttribute()
            {
                AttributeType = AttributeType.TweakedMod,
                Description = _textManager.GetText("ATTRIBUTE_TWEAKED")
            };
        }

        public IAttribute Get()
        {
            return TweakedAttribute;
        }
    }
}
