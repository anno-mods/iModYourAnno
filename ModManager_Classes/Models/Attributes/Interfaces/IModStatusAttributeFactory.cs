using Imya.Models.Attributes.Factories;

namespace Imya.Models.Attributes.Interfaces
{
    public interface IModStatusAttributeFactory
    {
        IAttribute Get(ModStatus status);
    }
}