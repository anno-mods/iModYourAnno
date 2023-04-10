using Imya.Models.Mods;
using Imya.Texts;
using Imya.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.Attributes
{
    public class CyclicDependencyAttributeFactory
    {
        private ITextManager _textManager;
        public CyclicDependencyAttributeFactory(ITextManager textManager)
        {
            _textManager = textManager;
        }

        public IAttribute Get(IEnumerable<Mod> context)
        {
            return new GenericModContextAttribute()
            {
                AttributeType = AttributeType.CyclicDependency,
                Description = new SimpleText(
                   String.Format(_textManager.GetText("ATTRIBUTE_CYCLIC_DEPENDENCY").Text,
                                String.Join(',', context.Select(x => $"[{x.Category}] {x.Name}")))),
                Context = context
            };
        }
    }
}
