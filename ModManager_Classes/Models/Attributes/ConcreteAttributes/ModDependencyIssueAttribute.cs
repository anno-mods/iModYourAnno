using Anno.EasyMod.Attributes;

namespace Imya.Models.Attributes
{
    public class ModDependencyIssueAttribute : IModAttribute
    {
        public string AttributeType { get; init; } = AttributeTypes.UnresolvedDependencyIssue;
        public IText Description { get; init; }

        bool IModAttribute.MultipleAllowed => true;

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
