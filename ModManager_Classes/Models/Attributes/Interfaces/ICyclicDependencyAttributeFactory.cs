using Imya.Models.Mods;

namespace Imya.Models.Attributes.Interfaces
{
    public interface ICyclicDependencyAttributeFactory
    {
        IAttribute Get(IEnumerable<Mod> context);
    }
}