using Anno.EasyMod.Attributes;
using Anno.EasyMod.Mods;
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
    public class CyclicDependencyAttributeFactory : ICyclicDependencyAttributeFactory
    {
        private ITextManager _textManager;
        public CyclicDependencyAttributeFactory(ITextManager textManager)
        {
            _textManager = textManager;
        }

        public IModAttribute Get(IEnumerable<IMod> context)
        {
            return new GenericModContextAttribute()
            {
                AttributeType = AttributeTypes.CyclicDependency,
                Description = new SimpleText(
                   string.Format(_textManager.GetText("ATTRIBUTE_CYCLIC_DEPENDENCY").Text,
                                string.Join(',', context.Select(x => $"[{x.Modinfo.Category}] {x.Name}")))),
                Context = context
            };
        }
    }
}
