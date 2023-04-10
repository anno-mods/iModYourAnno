using Imya.Models.Attributes;
using Imya.Models.Mods;
using Imya.Models.ModTweaker;
using Imya.Models.ModTweaker.DataModel;
using Imya.Models.ModTweaker.IO;
using Imya.Models.ModTweaker.Storage;
using System.Collections.Specialized;
using System.Threading;

namespace Imya.Validation
{
    //todo rework the hook system
    public class ModCollectionHooks
    {
        private ITweakRepository _tweakRepository;
        private ModTweaksLoader _tweaksLoader; 
        private SemaphoreSlim _tweaksave_sem;

        private List<IModValidator> validators = new(); 

        public ModCollectionHooks(ITweakRepository tweakRepository,
            ModTweaksLoader tweaksLoader)
        {
            _tweakRepository = tweakRepository;
            _tweaksLoader = tweaksLoader;
            _tweaksave_sem = new SemaphoreSlim(1);
        }

        public void HookTo(ModCollection mods)
        {
            mods.CollectionChanged += ValidateOnChange;
        }

        public void AddHook(IModValidator validator) 
        {
            validators.Add(validator);
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

        private void UpdateWithTweak(Mod mod)
        {
            mod.Attributes.RemoveAttributesByType(AttributeType.TweakedMod);
            if (!_tweakRepository.IsStored(mod.FolderName))
                return;

            // TODO double access is unprotected
            // TODO all validation should be offloaded to async, not tweaks individually
            Task.Run(() =>
            {
                ModTweaks tweaks = new();
                _tweaksLoader.Load(mod);
                _tweaksave_sem.Wait();
                tweaks.Save();
                _tweaksave_sem.Release();
            });
        }
    }
}
