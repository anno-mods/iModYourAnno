using Imya.Models.Attributes;
using Imya.Models.Attributes.Factories;
using Imya.Models.Mods;
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
        public bool IsEmpty => _tweakerFiles == null || _tweakerFiles.Count == 0;

        public ObservableCollection<TweakerFile>? TweakerFiles
        {
            get => _tweakerFiles;
            set
            {
                _tweakerFiles = value;
                OnPropertyChanged(nameof(TweakerFiles));
                OnPropertyChanged(nameof(IsEmpty));
            }
        }
        private ObservableCollection<TweakerFile>? _tweakerFiles = null;

        private Mod? _mod = null;

        public string ModBaseName { get; init; }

        public ModTweaks(string baseName, IEnumerable<TweakerFile> tweakerFiles)
        {
            ModBaseName = baseName;
            TweakerFiles = new ObservableCollection<TweakerFile>(tweakerFiles);
        }

        public ModTweaks()
        {

        }
    }
}
