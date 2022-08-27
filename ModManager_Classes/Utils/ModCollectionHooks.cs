using Imya.Models;
using Imya.Models.Attributes;
using Imya.Models.ModTweaker;
using Imya.Utils.Validation;

namespace Imya.Utils
{
    public class ModCollectionHooks
    {
        private ModCollectionHooks()
        {
            if (ModCollection.Global is null)
                return; // TODO should not be possible, but make it noticable somehow

            ModCollection.Global.CollectionChanged += ValidateOnChange;
        }

        private void ValidateOnChange(ModCollection.CollectionChangeAction action, IEnumerable<Mod> mods)
        {
            var validators = new IModValidator[]
            { 
                new ModContentValidator(),
                new ModCompatibilityValidator()
            };

            foreach (var mod in mods)
            {
                foreach (var validator in validators)
                {
                    validator.Validate(mod);
                }

                UpdateWithTweak(mod);
            }
        }

        public static void Initialize()
        {
            new ModCollectionHooks();
        }

        private static void UpdateWithTweak(Mod mod)
        {
            mod.Attributes.RemoveAttributesByType(AttributeType.TweakedMod);
            if (!TweakStorageShelf.Global.IsStored(mod.FolderName)) return;

            Task.Run(() =>
            {
                ModTweaks tweaks = new ModTweaks();
                tweaks.Load(mod);
                tweaks.Save();
            });
        }
    }
}
