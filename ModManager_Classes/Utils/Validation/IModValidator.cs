using Imya.Models;

namespace Imya.Utils.Validation
{
    public interface IModValidator
    {
        void Validate(Mod mod, ModCollection? collection = null);
    }
}
