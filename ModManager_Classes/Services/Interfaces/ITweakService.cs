using Anno.EasyMod.Mods;
using Imya.Models.ModTweaker.DataModel.Tweaking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Services.Interfaces
{
    public interface ITweakService
    {
        ModTweaks? Tweaks { get; }
        bool HasUnsavedChanges { get; set; }
        bool IsLoading { get; }
        bool IsSaving { get; }

        void Save();
        Task SaveAsync();
        void Unload();
        void Load(IMod mod, bool ClearCurrentWhileLoading = true);
    }
}
