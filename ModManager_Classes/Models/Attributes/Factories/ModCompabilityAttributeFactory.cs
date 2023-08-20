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
    public class ModCompabilityAttributeFactory : IModCompabilityAttributeFactory
    {
        private readonly ITextManager _textManager;
        public ModCompabilityAttributeFactory(ITextManager textManager)
        {
            _textManager = textManager;
        }

        public IModAttribute Get(IEnumerable<IMod> context)
        {
            return new GenericModContextAttribute()
            {
                AttributeType = AttributeTypes.ModCompabilityIssue,
                Description = new SimpleText(
                   string.Format(_textManager.GetText("ATTRIBUTE_COMPABILITYERROR").Text,
                                string.Join("\n - ", context.Select(x => $"[{x.Modinfo.Category}] {x.Name}")))),
                Context = context
            };
        }
    }
}
