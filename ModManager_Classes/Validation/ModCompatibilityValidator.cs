using Imya.Models.Attributes;
using Imya.Models.Attributes.Interfaces;
using Imya.Models.ModMetadata.ModinfoModel;
using Imya.Models.Mods;
using System.Collections.Immutable;
using System.Collections.Specialized;

namespace Imya.Validation
{
    public class ModCompatibilityValidator : IModValidator
    {
        private readonly IModCompabilityAttributeFactory _compabilityAttributeFactory;

        public ModCompatibilityValidator(IModCompabilityAttributeFactory factory) 
        {
            _compabilityAttributeFactory = factory;
        }

        public void Validate(IEnumerable<Mod> changed, IReadOnlyCollection<Mod> all, NotifyCollectionChangedAction changedAction)
        {
            foreach (var mod in all)
                ValidateSingle(mod, all);
        }

        private void ValidateSingle(Mod mod, IReadOnlyCollection<Mod> collection)
        {
            mod.Attributes.RemoveAttributesByType(AttributeType.ModCompabilityIssue);

            // skip dependency check if inactive or standalone
            if (!mod.IsActiveAndValid || collection is null)
                return;

            var incompatibles = GetIncompatibleMods(mod.Modinfo, collection);
            if (incompatibles.Any())
                mod.Attributes.AddAttribute(_compabilityAttributeFactory.Get(incompatibles));
        }

        private IEnumerable<Mod> GetIncompatibleMods(Modinfo modinfo, IReadOnlyCollection<Mod> collection)
        {
            if (collection is null || modinfo.IncompatibleIds is null || modinfo.ModID is null) 
                yield break;
            
            foreach (var inc in modinfo.IncompatibleIds)
            {
                var incompatibles = collection.Where(x => x.Modinfo.ModID is not null && x.Modinfo.ModID.Equals(inc) && x.IsActiveAndValid);
                foreach (var result in incompatibles)
                    yield return result;
            }
        }

    }
}
