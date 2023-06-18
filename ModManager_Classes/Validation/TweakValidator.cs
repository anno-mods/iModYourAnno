using Imya.Models.Attributes;
using Imya.Models.Attributes.Interfaces;
using Imya.Models.Mods;
using Imya.Models.ModTweaker.DataModel.Storage;
using Imya.Models.ModTweaker.IO;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Validation
{
    public class TweakValidator : IModValidator 
    {
        private ITweakRepository _tweakRepository;
        private SemaphoreSlim _tweaksave_sem; 
        private ModTweaksLoader _tweaksLoader;
        private ModTweaksExporter _tweaksExporter;
        private ITweakedAttributeFactory _tweakedAttributeFactory;

        public TweakValidator(
            ITweakRepository tweakRepository, 
            ModTweaksLoader tweaksLoader,
            ModTweaksExporter tweaksExporter,
            ITweakedAttributeFactory tweakedAttributeFactory) 
        {
            _tweakRepository = tweakRepository;
            _tweaksave_sem = new SemaphoreSlim(1);
            _tweaksLoader = tweaksLoader;
            _tweaksExporter = tweaksExporter;
            _tweakedAttributeFactory = tweakedAttributeFactory;
        }

        public void Validate(IEnumerable<Mod> changed, IReadOnlyCollection<Mod> all, NotifyCollectionChangedAction changedAction)
        {

            if (changedAction == NotifyCollectionChangedAction.Reset
                || changedAction == NotifyCollectionChangedAction.Add)
            {
                foreach (var mod in changed)
                {
                    UpdateWithTweak(mod);
                }
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
                var tweaks = _tweaksLoader.Load(mod);
                if (tweaks is not null && !tweaks.IsEmpty)
                {
                    mod.Attributes.AddAttribute(_tweakedAttributeFactory.Get());
                    _tweaksave_sem.Wait();
                    _tweaksExporter.Save(tweaks);
                    _tweaksave_sem.Release();
                }
            });
        }
    }
}
