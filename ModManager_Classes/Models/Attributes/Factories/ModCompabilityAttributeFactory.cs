using Imya.Models.Attributes.Interfaces;
using Imya.Models.Mods;
using Imya.Texts;
using Imya.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.Attributes.Factories
{
    public class ModCompabilityAttributeFactory : IModCompabilityAttributeFactory
    {
        private readonly ITextManager _textManager;
        public ModCompabilityAttributeFactory(ITextManager textManager)
        {
            _textManager = textManager;
        }

        public IAttribute Get(IEnumerable<Mod> context)
        {
            return new GenericModContextAttribute()
            {
                AttributeType = AttributeType.ModCompabilityIssue,
                Description = new SimpleText(
                   string.Format(_textManager.GetText("ATTRIBUTE_COMPABILITYERROR").Text,
                                string.Join("\n - ", context.Select(x => $"[{x.Category}] {x.Name}")))),
                Context = context
            };
        }
    }
}
