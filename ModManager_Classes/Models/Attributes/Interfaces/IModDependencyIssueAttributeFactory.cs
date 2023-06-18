using Imya.Models.Mods;

namespace Imya.Models.Attributes.Interfaces
{
    public interface IModDependencyIssueAttributeFactory
    {
        IAttribute Get(IEnumerable<String> context);
    }
}