using Imya.Models;

namespace Imya.Validation
{
    public interface IModValidator
    {
        void Validate(IEnumerable<Mod> changed, IReadOnlyCollection<Mod> all);
    }
}
