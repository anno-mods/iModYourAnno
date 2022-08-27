using Imya.Models;
using Imya.Models.Attributes;
using Imya.Models.ModMetadata;

namespace Imya.Utils.Validation
{
    public class ModCompatibilityValidator : IModValidator
    {
        public void Validate(Mod mod, ModCollection? collection)
        {
            mod.Attributes.RemoveAttributesByType(AttributeType.UnresolvedDependencyIssue);
            mod.Attributes.RemoveAttributesByType(AttributeType.ModCompabilityIssue);

            // skip dependency check if inactive or standalone
            if (!mod.IsActive || collection is null)
                return;

            var unresolvedDeps = GetUnresolvedDependencies(mod.Modinfo, collection);
            if (unresolvedDeps.Any())
                mod.Attributes.AddAttribute(new ModDependencyIssueAttribute(unresolvedDeps));

            var incompatibles = GetIncompatibleMods(mod.Modinfo, collection);
            if (incompatibles.Any())
                mod.Attributes.AddAttribute(new ModCompabilityIssueAttribute(incompatibles));
        }

        private static IEnumerable<string> GetUnresolvedDependencies(Modinfo modinfo, ModCollection collection)
        {
            if (modinfo.ModDependencies is null)
                yield break;

            foreach (var dep in modinfo.ModDependencies)
            {
                if (!collection.Mods.Any(x => x.Modinfo.ModID is not null && x.Modinfo.ModID.Equals(dep) && x.IsActive))
                    yield return dep;
            }
        }

        private static IEnumerable<Mod> GetIncompatibleMods(Modinfo modinfo, ModCollection collection)
        {
            if (collection is null || modinfo.IncompatibleIds is null || modinfo.ModID is null) 
                yield break;
            
            foreach (var inc in modinfo.IncompatibleIds)
            {
                var incompatibles = collection.Mods.Where(x => x.Modinfo.ModID is not null && x.Modinfo.ModID.Equals(inc) && x.IsActive);
                foreach (var result in incompatibles)
                    yield return result;
            }
        }
    }
}
