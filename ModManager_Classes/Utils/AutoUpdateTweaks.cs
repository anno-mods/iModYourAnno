using Imya.Models;
using Imya.Models.ModTweaker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Utils
{
    public class AutoUpdateTweaks
    {
        public AutoUpdateTweaks() {
            ModCollection.Global!.ModAdded += UpdateWithTweak;
        }

        public void UpdateWithTweak(Mod m)
        {
            if (!TweakStorageShelf.Global.IsStored(m.FolderName)) return;

            ModTweaks tweaks = new ModTweaks();
            tweaks.Load(m);
            tweaks.Save();
        }
    }
}
