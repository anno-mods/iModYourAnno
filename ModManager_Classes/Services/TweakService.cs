using Anno.EasyMod.Mods;
using Imya.Models.ModTweaker.DataModel.Tweaking;
using Imya.Models.ModTweaker.IO;
using Imya.Models.NotifyPropertyChanged;
using Imya.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Services
{
    public class TweakService : PropertyChangedNotifier, ITweakService
    {
        private readonly ModTweaksLoader _loader;
        private readonly ModTweaksExporter _exporter;

        public TweakService(ModTweaksLoader loader, ModTweaksExporter exporter)
        {
            _loader = loader;
            _exporter = exporter;
        }

        public ModTweaks? Tweaks
        {
            get => _tweaks;
            private set
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

        public bool IsSaving
        {
            get => _isSaving;
            private set => SetProperty(ref _isSaving, value);
        }
        private bool _isSaving;

        public bool IsLoading
        {
            get => _isLoading;
            private set => SetProperty(ref _isLoading, value);
        }
        private bool _isLoading = false;

        public void Save()
        {
            HasUnsavedChanges = false;
            var tweaks = Tweaks;
            IsSaving = true;
            ThreadPool.QueueUserWorkItem(o =>
            {
                _exporter.Save(tweaks);
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
            await Task.Run(() => _exporter.Save(tweaks));
            IsSaving = false;
        }

        public void Load(IMod mod, bool ClearCurrentWhileLoading = true)
        {
            IsLoading = true;
            HasUnsavedChanges = false;
            // make sure everything is secure from access from other threads
            var currentTweaks = Tweaks;
            if (ClearCurrentWhileLoading)
                Tweaks = null;
            ThreadPool.QueueUserWorkItem(o =>
            {
                var tweaks = mod is not null ?
                    _loader.Load(mod)
                    : new ModTweaks();
                Tweaks = tweaks;
                IsLoading = false;
            });
        }

    }
}
