using Anno.EasyMod.Attributes;

namespace Imya.Models.Attributes.Interfaces
{
    public interface IModDependencyIssueAttributeFactory
    {
        IModAttribute Get(IEnumerable<String> context);
    }
}