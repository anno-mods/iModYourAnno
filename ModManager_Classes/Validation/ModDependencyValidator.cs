using Imya.Models.Attributes;
using Anno.EasyMod.Metadata;
using Imya.Models.Attributes.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anno.EasyMod.Mods;

namespace Imya.Validation
{
    public class ModDependencyValidator : IModValidator
    {
        private readonly IModDependencyIssueAttributeFactory _dependencyAttributeFactory;

        public ModDependencyValidator(IModDependencyIssueAttributeFactory factory)
        {
            _dependencyAttributeFactory = factory;
        }

        public void Validate(IEnumerable<IMod> changed, IReadOnlyCollection<IMod> all, NotifyCollectionChangedAction changedAction)
        {
            foreach (var mod in all)
                ValidateSingle(mod, all);
        }

        private void ValidateSingle(IMod mod, IReadOnlyCollection<IMod> collection)
        {
            mod.Attributes.RemoveByType(AttributeTypes.UnresolvedDependencyIssue);
            // skip dependency check if inactive or standalone
            if (!mod.IsActiveAndValid || collection is null)
                return;

            var unresolvedDeps = GetUnresolvedDependencies(mod.Modinfo, collection).ToArray();
            if (unresolvedDeps.Any())
                mod.Attributes.Add(_dependencyAttributeFactory.Get(unresolvedDeps));
        }

        private IEnumerable<string> GetUnresolvedDependencies(Modinfo modinfo, IReadOnlyCollection<IMod> collection)
        {
            if (modinfo.ModDependencies is null)
                yield break;

            foreach (var dep in modinfo.ModDependencies)
            {
                if (!collection.Any(x => x.Modinfo.ModID is not null
                    && (x.Modinfo.ModID.Equals(dep) || x.SubMods?.Where(submod => submod.ModID.Equals(dep)) is not null)
                    && x.IsActiveAndValid))
                    yield return dep;
            }
        }
    }
}
