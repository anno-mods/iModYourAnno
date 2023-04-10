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

        private string? ModBaseName;

        public ModTweaks(string baseName, IEnumerable<TweakerFile> tweakerFiles)
        {
            ModBaseName = baseName;
            TweakerFiles = new ObservableCollection<TweakerFile>(tweakerFiles);
        }

        public ModTweaks()
        {

        }

        public void Save()
        {
            if (TweakerFiles != null && TweakerFiles.Count > 0 && _mod != null)
            {
                foreach (var f in TweakerFiles)
                {
                    f.Save(_mod.FullModPath);
                }
                TweakStorage.Save(ModBaseName);

                if (_mod is not null && !_mod.Attributes.HasAttribute(AttributeType.TweakedMod))
                {
                    //this should be done in the hooks honestly
                    _mod.Attributes.AddAttribute(TweakedAttributeFactory.Get());
                }
            }


        }
    }
}
