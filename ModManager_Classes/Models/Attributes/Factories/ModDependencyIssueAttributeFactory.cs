using Anno.EasyMod.Attributes;
using Imya.Models.Attributes.Interfaces;
using Imya.Texts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.Attributes.Factories
{
    public class ModDependencyIssueAttributeFactory : IModDependencyIssueAttributeFactory
    {
        private ITextManager _textManager;
        public ModDependencyIssueAttributeFactory(ITextManager textManager)
        {
            _textManager = textManager;
        }

        public IModAttribute Get(IEnumerable<string> context)
        {
            String text = _textManager.GetText("ATTRIBUTE_MISSINGDEPENDENCY")?.Text ?? "";
            return new ModDependencyIssueAttribute()
            {
                AttributeType = AttributeTypes.UnresolvedDependencyIssue,
                Description = new SimpleText(string.Format(text, string.Join("\n - ", context))),
                UnresolvedDependencies = context
            };
        }
    }
}
