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
        public IText Description { get => new SimpleText($"Missing Dependencies: {String.Join(',', UnresolvedDependencies)}"); }
        public string Icon { get; } = "FileTree";
        public string Color { get; } = "Red";

        public IEnumerable<String> UnresolvedDependencies { get; }

        public ModDependencyIssueAttribute(IEnumerable<String> issues)
        {
            UnresolvedDependencies = issues;
        }
    }
}
