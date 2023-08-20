using Anno.EasyMod.Attributes;
using Anno.EasyMod.Mods;

namespace Imya.Models.Attributes.Interfaces
{
    public interface IModCompabilityAttributeFactory
    {
        IModAttribute Get(IEnumerable<IMod> context);
    }
}