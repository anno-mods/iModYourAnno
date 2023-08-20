using Anno.EasyMod.Attributes;
using Anno.EasyMod.Mods;

namespace Imya.Models.Attributes.Interfaces
{
    public interface IModReplacedByAttributeFactory
    {
        IModAttribute Get(IMod replacedBy);
    }
}