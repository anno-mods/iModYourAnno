using Imya.Models.ModTweaker.DataModel.Storage;
using Imya.Models.ModTweaker.DataModel.Tweaking;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.ModTweaker.IO
{
    //a mediator class between the modtweaks model and the tweakfilestorage model.
    public class TweakMediator
    {

        public ModTweaksStorageModel SaveToRepository(ModTweaks tweaks)
        {
            ModTweaksStorageModel storage = new ModTweaksStorageModel();
            foreach (var file in tweaks.TweakerFiles!)
            {
                var tweak = GetTweak(file);
                storage.Tweaks.Add(file.SourceFilename, tweak);
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
