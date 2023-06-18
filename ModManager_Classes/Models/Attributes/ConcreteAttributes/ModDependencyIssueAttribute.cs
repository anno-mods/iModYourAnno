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
        public AttributeType AttributeType { get; init; } = AttributeType.UnresolvedDependencyIssue;
        public IText Description { get; init; }

        bool IAttribute.MultipleAllowed => true;

        public IEnumerable<String> UnresolvedDependencies { get; init; }

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
