using Anno.EasyMod.Attributes;
using Imya.Models.Attributes.Factories;

namespace Imya.Models.Attributes.Interfaces
{
    public interface IModStatusAttributeFactory
    {
        IModAttribute Get(ModStatus status);
    }
}