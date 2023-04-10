using Imya.Models.Attributes.Interfaces;
using Imya.Texts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                AttributeType = AttributeType.ModContentInSubfolder,
                Description = _textManager.GetText("ATTRIBUTE_MODCONTENTSUBFOLDER")
            };
        }

        public IAttribute Get() => Subfolder;
    }
}
