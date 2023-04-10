using Imya.Models.Attributes;
using Imya.Models.Attributes.Interfaces;
using Imya.Models.ModMetadata.ModinfoModel;
using Imya.Models.Mods;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Validation
{
    public class ModDependencyValidator : IModValidator
    {
        private readonly IModDependencyIssueAttributeFactory _dependencyAttributeFactory;

        public ModDependencyValidator(IModDependencyIssueAttributeFactory factory)
        {
            _dependencyAttributeFactory = factory;
        }

        public void Validate(IEnumerable<Mod> changed, IReadOnlyCollection<Mod> all, NotifyCollectionChangedAction changedAction)
        {
            foreach (var mod in all)
                ValidateSingle(mod, all);
        }

        private void ValidateSingle(Mod mod, IReadOnlyCollection<Mod> collection)
        {
            mod.Attributes.RemoveAttributesByType(AttributeType.UnresolvedDependencyIssue);
            // skip dependency check if inactive or standalone
            if (!mod.IsActiveAndValid || collection is null)
                return;

            var unresolvedDeps = GetUnresolvedDependencies(mod.Modinfo, collection).ToArray();
            if (unresolvedDeps.Any())
                mod.Attributes.AddAttribute(_dependencyAttributeFactory.Get(unresolvedDeps));
        }

        private IEnumerable<string> GetUnresolvedDependencies(Modinfo modinfo, IReadOnlyCollection<Mod> collection)
        {
            if (modinfo.ModDependencies is null)
                yield break;

            foreach (var dep in modinfo.ModDependencies)
            {
                if (!collection.Any(x => x.Modinfo.ModID is not null && x.Modinfo.ModID.Equals(dep) && x.IsActiveAndValid))
                    yield return dep;
            }
        }
    }
}
