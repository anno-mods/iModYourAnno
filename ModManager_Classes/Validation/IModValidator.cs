using Anno.EasyMod.Mods;
using System.Collections.Specialized;

namespace Imya.Validation
{
    public interface IModValidator
    {
        void Validate(IEnumerable<IMod> changed, IReadOnlyCollection<IMod> all, NotifyCollectionChangedAction changedAction);
    }
}
