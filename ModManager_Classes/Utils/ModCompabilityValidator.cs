using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Imya.Models;
using Imya.Models.ModMetadata;

namespace Imya.Utils
{
    public struct ModCompabilityResult
    {
        public Mod CheckedMod;
        public IEnumerable<Mod>? IncompatibleMods;
        public IEnumerable<String>? UnresolvedDependencies;

        public bool HasIncompatibleMods { get => IncompatibleMods is not null; }
        public bool HasUnresolvedDependencies { get => UnresolvedDependencies is not null; }
    }

    public class ModCompabilityValidator
    {
        private readonly IEnumerable<Mod> mods;

        public ModCompabilityValidator()
        {
            mods = ModCollection.Global?.Mods ?? Array.Empty<Mod>();
        }

        public IEnumerable<ModCompabilityResult> RunCompabilityCheck()
        {
            foreach (var mod in mods)
            {
                var _unresolvedDependencies = GetUnresolvedDependencies(mod.Modinfo);
                var _incompatibleMods = GetIncompatibleModIds(mod.Modinfo);

                bool hasUnr = _unresolvedDependencies.Count() > 0;
                bool hasInc = _incompatibleMods.Count() > 0;

                if (hasInc || hasUnr)
                {
                    yield return new ModCompabilityResult
                    {
                        CheckedMod = mod,
                        UnresolvedDependencies = hasUnr ? _unresolvedDependencies : null,
                        IncompatibleMods = hasInc ? _incompatibleMods : null
                    };
                }
            }
        }

        public IEnumerable<String> GetUnresolvedDependencies(Modinfo modinfo)
        {
            if (modinfo.ModDependencies is null) yield break;

            foreach (var dep in modinfo.ModDependencies)
            {
                if (!mods.Any(x => x.Modinfo.ModID is not null && x.Modinfo.ModID.Equals(dep)))
                    yield return dep;
            }
        }

        public IEnumerable<Mod> GetIncompatibleModIds(Modinfo modinfo)
        {
            if (modinfo.IncompatibleIds is null || modinfo.ModID is null) yield break;
            foreach (var inc in modinfo.IncompatibleIds)
            {
                var Incompatibles = mods.Where(x => x.Modinfo.ModID is not null && x.Modinfo.ModID.Equals(inc));

                foreach (var result in Incompatibles)
                    yield return result;
            }
        }
    }
}
