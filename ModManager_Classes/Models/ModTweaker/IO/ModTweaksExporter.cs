using Imya.Models.Mods;
using Imya.Models.ModTweaker.DataModel.Storage;
using Imya.Models.ModTweaker.DataModel.Tweaking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.ModTweaker.IO
{
    public class ModTweaksExporter
    {
        private readonly ITweakRepository _tweakRepository;
        public ModTweaksExporter(ITweakRepository tweakRepository) 
        {
            _tweakRepository = tweakRepository;
        }

        public void Save(ModTweaks tweaks)
        {
            var storageModel = ConvertToStorage(tweaks);
            _tweakRepository.UpdateStorage(storageModel, tweaks.ModBaseName);
        }

        private ModTweaksStorageModel ConvertToStorage(ModTweaks tweaks)
        {
            ModTweaksStorageModel storage = new ModTweaksStorageModel();
            if (tweaks.TweakerFiles is null)
                return new ModTweaksStorageModel();
            foreach (var file in tweaks.TweakerFiles!)
            {
                var tweak = GetTweak(file);
                storage.Tweaks.Add(file.FilePath, tweak);
            }
            return storage;
        }

        private TweakerFileStorageModel GetTweak(TweakerFile tweakerFile)
        {
            var tweak = new TweakerFileStorageModel();
            foreach (IExposedModValue expose in tweakerFile.Exposes)
                tweak.SetTweakValue(expose.ExposeID, expose.Value);
            return tweak;
        }
    }
}
