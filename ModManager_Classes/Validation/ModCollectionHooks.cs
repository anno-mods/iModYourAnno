using Imya.Models;
using Imya.Models.Attributes;
using Imya.Models.ModTweaker;
using System.Collections.Specialized;

namespace Imya.Validation
{
    public class ModCollectionHooks
    {
        private readonly IModValidator[] validators = new IModValidator[]
            {
                new ModContentValidator(),
                new ModCompatibilityValidator()
            };

        public ModCollectionHooks(ModCollection mods)
        {
            mods.CollectionChanged += ValidateOnChange;
        }

        private void ValidateOnChange(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (sender is not ModCollection collection)
                return;

            IEnumerable<Mod> changed = e.NewItems?.OfType<Mod>() ?? collection.Mods;
            foreach (var validator in validators)
                validator.Validate(changed, collection.Mods);

            if (e.Action == NotifyCollectionChangedAction.Reset
                || e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var mod in changed)
                    UpdateWithTweak(mod);
            }
        }

        private static void UpdateWithTweak(Mod mod)
        {
            mod.Attributes.RemoveAttributesByType(AttributeType.TweakedMod);
            if (!TweakStorageShelf.Global.IsStored(mod.FolderName))
                return;

            // TODO double access is unprotected
            // TODO all validation should be offloaded to async, not tweaks individually
            Task.Run(() =>
            {
                ModTweaks tweaks = new();
                tweaks.Load(mod);
                tweaks.Save();
            });
        }
    }
}
