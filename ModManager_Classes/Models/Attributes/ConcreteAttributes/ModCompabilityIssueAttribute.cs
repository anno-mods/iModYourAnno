using Imya.Utils;

namespace Imya.Models.Attributes
{
    public class ModCompabilityIssueAttribute : IAttribute
    {
        public AttributeType AttributeType { get; } = AttributeType.ModCompabilityIssue;
        public IText Description { get => new SimpleText(String.Format(TextManager.Instance.GetText("ATTRIBUTE_COMPABILITYERROR").Text, String.Join(',', CompabilityIssues.Select(x => $"{x.Category} {x.Name}")))); }

        public IEnumerable<Mod> CompabilityIssues { get; }

        public ModCompabilityIssueAttribute(IEnumerable<Mod> issues)
        { 
            CompabilityIssues = issues;
        }

        public ModCompabilityIssueAttribute()
        { 
            CompabilityIssues = Enumerable.Empty<Mod>();
        }
    }
}
