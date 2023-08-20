using Anno.EasyMod.Mods;
using Imya.Models.Attributes;
using Imya.Models.Attributes.Factories;
using Imya.Models.NotifyPropertyChanged;
using Imya.Services;
using System.Collections.ObjectModel;

namespace Imya.Models.ModTweaker.DataModel.Tweaking
{
    /// <summary>
    /// Represents all Tweaker Files in a Mod at once
    /// </summary>
    public class ModTweaks : PropertyChangedNotifier
    {
        public bool IsEmpty => _tweakerFiles == null || _tweakerFiles.Count() == 0;

        public IEnumerable<TweakerFile>? TweakerFiles
        {
            get => _tweakerFiles ?? Enumerable.Empty<TweakerFile>();
            set
            {
                _tweakerFiles = value;
                OnPropertyChanged(nameof(TweakerFiles));
                OnPropertyChanged(nameof(IsEmpty));
            }
        }
        private IEnumerable<TweakerFile>? _tweakerFiles = null;

        private IMod? _mod = null;

        public string ModBaseName { get => _mod.FolderName; }
        public string ModBasePath { get => _mod.FullModPath; }

        public ModTweaks(IMod mod, IEnumerable<TweakerFile> tweakerFiles)
        {
            _mod = mod;
            TweakerFiles = new ObservableCollection<TweakerFile>(tweakerFiles);
        }

        public ModTweaks()
        {

        }
    }
}
