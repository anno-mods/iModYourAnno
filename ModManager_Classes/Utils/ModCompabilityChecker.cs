using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Imya.Models;

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

    public class ModCompabilityChecker
    {
        private IEnumerable<Mod> mods;

        public ModCompabilityChecker()
        {
            mods = ModDirectoryManager.Instance.GetMods();
        }

        public IEnumerable<ModCompabilityResult> RunCompabilityCheck()
        {
            foreach (var mod in mods)
            {
                var _unresolvedDependencies = GetUnresolvedDependencies(mod);
                var _incompatibleMods = GetIncompatibleModIds(mod);

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

        public IEnumerable<String> GetUnresolvedDependencies(Mod mod)
        {
            if (mod.ModDependencies is not null)
            {
                foreach (var dep in mod.ModDependencies)
                {
                    if (!mods.Any(x => x.HasModID && x.ModID.Equals(dep)))
                    {
                        yield return dep;
                    }
                }
            }
        }

        public IEnumerable<Mod> GetIncompatibleModIds(Mod mod)
        {
            if (mod.IncompatibleModIDs is not null && mod.HasModID )
            {
                foreach (var inc in mod.IncompatibleModIDs)
                {
                    var Incompatibles = mods.Where(x => x.HasModID && x.ModID.Equals(inc));

                    foreach (var result in Incompatibles)
                    {
                        yield return result;
                    }
                }
            }
        }
    }
}
