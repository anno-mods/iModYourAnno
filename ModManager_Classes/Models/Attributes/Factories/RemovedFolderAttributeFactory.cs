using Anno.EasyMod.Attributes;
using Imya.Models.Attributes.Interfaces;
using Imya.Texts;
using Imya.Utils;

namespace Imya.Models.Attributes.Factories
{
    public class RemovedFolderAttributeFactory : IRemovedFolderAttributeFactory
    {
        private readonly ITextManager _textManager;
        GenericAttribute RemovedFolderAttribute;
        public RemovedFolderAttributeFactory(ITextManager textManager)
        {
            _textManager = textManager;

            RemovedFolderAttribute = new GenericAttribute()
            {
                AttributeType = AttributeTypes.IssueModRemoved,
                //Description = TextManager.Instance.GetText("ATTRIBUTE_TWEAKED")
                Description = new SimpleText("This mod has been removed by another program")
            };
        }

        public IModAttribute Get()
        {
            return RemovedFolderAttribute;
        }
    }
}
