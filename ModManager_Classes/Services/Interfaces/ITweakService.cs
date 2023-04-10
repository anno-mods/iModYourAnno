using Imya.Models.Mods;
using Imya.Models.ModTweaker.DataModel;
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
        void Load(Mod mod, bool ClearCurrentWhileLoading = true);
    }
}
