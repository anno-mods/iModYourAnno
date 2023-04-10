using Imya.Models.Mods;
using Imya.Models.ModTweaker.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.ModTweaker.IO
{
    [Obsolete]
    public interface ITweakMediator
    {
        void ApplyStoredValues(ModTweaks tweaks, IModTweaksStorageModel stored);
        ModTweaksStorageModel SaveToRepository(ModTweaks tweaks);
    }
}
