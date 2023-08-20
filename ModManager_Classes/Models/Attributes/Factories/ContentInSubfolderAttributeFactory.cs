using Anno.EasyMod.Attributes;
using Imya.Models.Attributes.Interfaces;
using Imya.Texts;

namespace Imya.Models.Attributes.Factories
{
    public class ContentInSubfolderAttributeFactory : IContentInSubfolderAttributeFactory
    {
        private readonly ITextManager _textManager;
        private GenericAttribute Subfolder;

        public ContentInSubfolderAttributeFactory(ITextManager textManager)
        {
            _textManager = textManager;

            Subfolder = new GenericAttribute()
            {
                AttributeType = AttributeTypes.ModContentInSubfolder,
                Description = _textManager.GetText("ATTRIBUTE_MODCONTENTSUBFOLDER")
            };
        }

        public IModAttribute Get() => Subfolder;
    }
}
