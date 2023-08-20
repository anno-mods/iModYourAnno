using Anno.EasyMod.Attributes;
using Imya.Models.Attributes.Interfaces;
using Imya.Texts;

namespace Imya.Models.Attributes.Factories
{
    public class MissingModinfoAttributeFactory : IMissingModinfoAttributeFactory
    {
        private readonly ITextManager _textManager;
        private GenericAttribute MissingModinfoAttribute;

        public MissingModinfoAttributeFactory(ITextManager textManager)
        {
            _textManager = textManager;
        }

        public IModAttribute Get()
        {
            return new GenericAttribute()
            {
                AttributeType = AttributeTypes.MissingModinfo,
                Description = _textManager.GetText("ATTRIBUTE_NOMODINFO")
            };
        }
    }
}
