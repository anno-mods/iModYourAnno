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
        public bool HasIssues = true;
        public IEnumerable<Mod>? IncompatibleMods;
        public IEnumerable<String>? UnresolvedDependencies;

        public bool HasIncompatibleMods { get => IncompatibleMods is not null; }
        public bool HasUnresolvedDependencies { get => UnresolvedDependencies is not null; }
    }

    public class ModCompabilityValidator
    {
        private ModCollection? _context => ModCollection.Global;

        public ModCompabilityValidator()
        {

        }

        public IEnumerable<String> GetUnresolvedDependencies(Mod mod) => mod.IsActive ? GetUnresolvedDependencies(mod.Modinfo) : Enumerable.Empty<String>();

        public IEnumerable<Mod> GetIncompatibleMods(Mod mod) => mod.IsActive ? GetIncompatibleMods(mod.Modinfo) : Enumerable.Empty<Mod>();


        public IEnumerable<String> GetUnresolvedDependencies(Modinfo modinfo)
        {
            if (modinfo.ModDependencies is null) yield break;

            foreach (var dep in modinfo.ModDependencies)
            {
                if (_context is not ModCollection context) continue;

                if (!context.Mods.Any(x => x.Modinfo.ModID is not null && x.Modinfo.ModID.Equals(dep) && x.IsActive))
                    yield return dep;
            }
        }

        public IEnumerable<Mod> GetIncompatibleMods(Modinfo modinfo)
        {
            if (modinfo.IncompatibleIds is null || modinfo.ModID is null) yield break;
            foreach (var inc in modinfo.IncompatibleIds)
            {
                var Incompatibles = _context?.Mods.Where(x => x.Modinfo.ModID is not null && x.Modinfo.ModID.Equals(inc) && x.IsActive);

                foreach (var result in Incompatibles)
                    yield return result;
            }
        }
    }
}
