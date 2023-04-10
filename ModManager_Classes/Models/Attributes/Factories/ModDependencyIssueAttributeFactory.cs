using Imya.Models.Attributes.Interfaces;
using Imya.Models.Mods;
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

        public IAttribute Get(IEnumerable<string> context)
        {
            return new ModDependencyIssueAttribute()
            {
                AttributeType = AttributeType.UnresolvedDependencyIssue,
                Description = new SimpleText(string.Format(_textManager.GetText("ATTRIBUTE_MISSINGDEPENDENCY").Text, string.Join(',', context))),
                UnresolvedDependencies = context
            };
        }
    }
}
