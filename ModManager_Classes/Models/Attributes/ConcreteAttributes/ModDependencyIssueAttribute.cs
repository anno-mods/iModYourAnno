using Imya.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.Attributes
{
    public class ModDependencyIssueAttribute : IAttribute
    {
        public AttributeType AttributeType { get; } = AttributeType.UnresolvedDependencyIssue;
        public IText Description { get => new SimpleText(String.Format(TextManager.Instance.GetText("ATTRIBUTE_MISSINGDEPENDENCY").Text, String.Join(',', UnresolvedDependencies))); }

        bool IAttribute.MultipleAllowed => true;

        public IEnumerable<String> UnresolvedDependencies { get; }

        public ModDependencyIssueAttribute(IEnumerable<String> issues)
        {
            UnresolvedDependencies = issues;
        }

        public ModDependencyIssueAttribute()
        {
            UnresolvedDependencies = Enumerable.Empty<String>();
        }
    }
}
