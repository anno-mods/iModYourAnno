using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.Attributes
{
    public class ModCompabilityIssueAttribute : IAttribute
    {
        public AttributeType AttributeType { get; } = AttributeType.ModCompabilityIssue;
        public IText Description { get => new SimpleText($"Compability Issues: {String.Join(',', CompabilityIssues.Select(x => $"{x.Category} {x.Name}" ))}"); }

        public IEnumerable<Mod> CompabilityIssues { get; }

        public ModCompabilityIssueAttribute(IEnumerable<Mod> issues)
        { 
            CompabilityIssues = issues;
        }
    }
}
