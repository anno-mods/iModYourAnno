using Imya.Models.Attributes.Interfaces;
using Imya.Texts;
using Imya.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public IAttribute Get()
        {
            return new GenericAttribute()
            {
                AttributeType = AttributeType.MissingModinfo,
                Description = _textManager.GetText("ATTRIBUTE_NOMODINFO")
            };
        }
    }
}
