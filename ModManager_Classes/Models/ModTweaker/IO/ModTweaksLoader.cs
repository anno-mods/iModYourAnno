﻿using Imya.Models.Mods;
using Imya.Models.ModTweaker.DataModel.Storage;
using Imya.Models.ModTweaker.DataModel.Tweaking;

namespace Imya.Models.ModTweaker.IO
{
    public class ModTweaksLoader
    {
        private readonly ITweakRepository _tweakRepository;

        public ModTweaksLoader(ITweakRepository tweakRepository) 
        {
            _tweakRepository = tweakRepository;
        }

        public ModTweaks? Load(Mod mod)
        {

            var files = mod.GetFilesWithExtension("xml")
                            .Where(x => !x.EndsWith("imyatweak.include.xml"))
                            .Select(x => Path.GetRelativePath(mod.FullModPath, x))
                            .ToArray();

            var list = new List<TweakerFile>();
            foreach (string file in files)
            {
                if (TweakerFile.TryInit(mod.FullModPath, file, out var tweakerFile))
                    list.Add(tweakerFile);
            }

            var tweaks = new ModTweaks(mod, list);
            var storedtweaks = _tweakRepository.Get(tweaks.ModBaseName);

            ApplyStoredValues(tweaks, storedtweaks);
            return tweaks;
        }

        private void ApplyStoredValues(ModTweaks tweaks, ModTweaksStorageModel stored)
        {
            foreach (TweakerFile file in tweaks.TweakerFiles!)
            {
                var tweak = stored.GetTweak(file.FilePath);
                if (tweak is not null)
                    ApplyToTweakerFile(file, tweak);
            }
        }

        private void ApplyToTweakerFile(TweakerFile file, TweakerFileStorageModel tweak)
        {
            foreach (IExposedModValue expose in file.Exposes)
            {
                if (!tweak.HasStoredValue(expose.ExposeID))
                    continue;

                if (expose is ExposedToggleModValue toggleExpose)
                {
                    if (!Boolean.TryParse(tweak.GetTweakValue(expose.ExposeID), out bool result))
                    {
                        result = false; 
                    }
                    toggleExpose.IsTrue = result;
                    continue; 
                }
                expose.Value = tweak.GetTweakValue(expose.ExposeID)!;
            }
        }
    }
}
