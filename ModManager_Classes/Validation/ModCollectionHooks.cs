using Imya.Models.Attributes;
using Imya.Models.Mods;
using Imya.Models.ModTweaker;
using Imya.Models.ModTweaker.DataModel.Storage;
using Imya.Models.ModTweaker.DataModel.Tweaking;
using Imya.Models.ModTweaker.IO;
using System.Collections.Specialized;
using System.Threading;

namespace Imya.Validation
{
    //todo rework the hook system
    public class ModCollectionHooks
    {
        private List<IModValidator> validators = new(); 

        public ModCollectionHooks()
        {
            
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
                validator.Validate(changed, collection.Mods, e.Action);
        }

    }
}
