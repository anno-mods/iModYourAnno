using Imya.Models;
using Imya.Models.Attributes;
using Imya.Models.ModTweaker;

namespace Imya.Utils
{
    public class ModCollectionHooks
    {
        ModCompabilityValidator compabilityValidator = new ModCompabilityValidator();

        private ModCollectionHooks() {
            ModCollection.Global!.ModAdded += UpdateWithTweak;
            ModCollection.Global!.Updated += UpdateCompabilityCheck;
        }

        public static void Initialize()
        {
            new ModCollectionHooks();
        }

        public void UpdateWithTweak(Mod m)
        {
            m.Attributes.RemoveAttributesByType(AttributeType.TweakedMod);
            if (!TweakStorageShelf.Global.IsStored(m.FolderName)) return;

            Task.Run(() =>
            {
                ModTweaks tweaks = new ModTweaks();
                tweaks.Load(m);
                tweaks.Save();
            });
        }

        public void UpdateCompabilityCheck()
        {
            foreach (Mod mod in ModCollection.Global!.Mods)
            {
                UpdateCompability(mod);
            }
        }

        private void UpdateCompability(Mod m)
        {
            var unresolvedDeps = compabilityValidator.GetUnresolvedDependencies(m);
            m.Attributes.RemoveAttributesByType(AttributeType.UnresolvedDependencyIssue);
            if (unresolvedDeps.Any())
                m.Attributes.AddAttribute(new ModDependencyIssueAttribute(unresolvedDeps));

            var incompatibles = compabilityValidator.GetIncompatibleMods(m);
            m.Attributes.RemoveAttributesByType(AttributeType.ModCompabilityIssue);
            if (incompatibles.Any())
                m.Attributes.AddAttribute(new ModCompabilityIssueAttribute(incompatibles));

        }
    }
}
