using Anno.EasyMod.Mods;
using Imya.Models.Attributes;
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
        private IModCollection _mods; 

        public ModCollectionHooks()
        {
            
        }

        public void HookTo(IModCollection mods)
        {
            mods.CollectionChanged += ValidateOnChange;
            _mods = mods; 
        }

        public void HookTo(IDlcOwnershipChanged dlcOwnership)
        {
            dlcOwnership.DlcSettingChanged += ValidateOnDlcChange;
        }

        public void AddHook(IModValidator validator) 
        {
            validators.Add(validator);
        }

        private void ValidateOnChange(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (sender is not IModCollection collection)
                return;

            IEnumerable<IMod> changed = e.NewItems?.OfType<IMod>() ?? collection.Mods;
            foreach (var validator in validators)
                validator.Validate(changed, collection.Mods, e.Action);
        }

        private void ValidateOnDlcChange()
        {
            foreach (var validator in validators)
            {
                validator.Validate(Enumerable.Empty<IMod>(), _mods.Mods, NotifyCollectionChangedAction.Move);
            }
        }

    }
}
