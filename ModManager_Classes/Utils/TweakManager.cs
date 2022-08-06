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

        public ModTweaks Tweaks
        {
            get => _tweaks;
            set
            {
                _tweaks = value;
                OnPropertyChanged(nameof(Tweaks));
            }
        }
        private ModTweaks _tweaks = new();

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

        public void Save()
        {
            HasUnsavedChanges = false;
            var tweaks = Tweaks;
            ThreadPool.QueueUserWorkItem(o =>
            {
                tweaks.Save();
            });
        }

        public async Task SaveAsync()
        {
            HasUnsavedChanges = false;
            var tweaks = Tweaks;
            await Task.Run(() => tweaks.Save());
        }

        public void Load(Mod mod, bool ClearCurrentWhileLoading = true)
        {
            // make sure everything is secure from access from other threads
            var currentTweaks = Tweaks;
            if(ClearCurrentWhileLoading)
                Tweaks = new();
            ThreadPool.QueueUserWorkItem(o =>
            {
                ModTweaks tweaks = new();

                if (mod is not null)
                    tweaks.Load(mod);
                Tweaks = tweaks;
                HasUnsavedChanges = false;
            });
        }

    }
}
