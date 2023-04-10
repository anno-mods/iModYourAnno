using Imya.Models.Mods;

namespace Imya.Models.Attributes.Interfaces
{
    public interface IModCompabilityAttributeFactory
    {
        IAttribute Get(IEnumerable<Mod> context);
    }
}