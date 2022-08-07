using Imya.Models;
using Imya.Models.ModTweaker;
using Imya.Models.NotifyPropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Utils
{
    public class TweakManager : PropertyChangedNotifier
    {
        public static TweakManager Instance { get; } = new TweakManager();

        public ModTweaks? Tweaks
        {
            get => _tweaks;
            set
            {
                _tweaks = value;
                OnPropertyChanged(nameof(Tweaks));
            }
        }
        private ModTweaks? _tweaks = new();

        public bool HasUnsavedChanges
        {
            get => _hasUnsavedChanges;
            set
            {
                _hasUnsavedChanges = value;
                OnPropertyChanged(nameof(HasUnsavedChanges));
            }
        }
        private bool _hasUnsavedChanges;

        private bool IsSaving; 

        public void Save()
        {
            HasUnsavedChanges = false;
            var tweaks = Tweaks;
            IsSaving = true;
            ThreadPool.QueueUserWorkItem(o =>
            {
                tweaks?.Save();
                IsSaving = false;
            });
        }

        public void Unload()
        {
            if (IsSaving) throw new InvalidOperationException("Cannot unload while saving!");
            Tweaks = null;
            HasUnsavedChanges = false;
        }

        public async Task SaveAsync()
        {

            IsSaving = true;
            HasUnsavedChanges = false;
            var tweaks = Tweaks;
            await Task.Run(() => tweaks?.Save());
            IsSaving = false;
        }

        public void Load(Mod mod, bool ClearCurrentWhileLoading = true)
        {
            HasUnsavedChanges = false;
            // make sure everything is secure from access from other threads
            var currentTweaks = Tweaks;
            if(ClearCurrentWhileLoading)
                Tweaks = null;
            ThreadPool.QueueUserWorkItem(o =>
            {
                ModTweaks tweaks = new();
                if (mod is not null)
                    tweaks.Load(mod);
                Tweaks = tweaks;
            });
        }

    }
}
