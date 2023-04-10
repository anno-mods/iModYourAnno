using Imya.Models.Mods;

namespace Imya.Models.Attributes.Interfaces
{
    public interface IModReplacedByAttributeFactory
    {
        IAttribute Get(Mod replacedBy);
    }
}