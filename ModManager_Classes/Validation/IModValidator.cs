using Imya.Models.Mods;
using System.Collections.Specialized;

namespace Imya.Validation
{
    public interface IModValidator
    {
        void Validate(IEnumerable<Mod> changed, IReadOnlyCollection<Mod> all, NotifyCollectionChangedAction changedAction);
    }
}
