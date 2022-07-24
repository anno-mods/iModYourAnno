using Imya.Models.Attributes;
using Imya.Models.NotifyPropertyChanged;
using Imya.Utils;
using System.Collections.ObjectModel;

namespace Imya.Models.ModTweaker
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

        private String? ModBaseName => _mod?.FolderName;

        public ITweakStorage TweakStorage;
        public void Load(Mod mod)
        {
            if (mod is null) return; 

            _mod = mod;
            var files = mod.GetFilesWithExtension("xml").Where(x => !x.EndsWith("imyatweak.xml")).Select(x => Path.GetRelativePath(mod.FullModPath, x)).ToArray();
            var list = new ObservableCollection<TweakerFile>();

            TweakStorage = TweakFileStorage.LoadOrCreate(ModBaseName!);
            foreach (string filename in files)
            {
                if (TweakerFile.TryInit(mod.FullModPath, filename, TweakStorage, out var file))
                {
                    list.Add(file);
                }
            }
            TweakerFiles = list;
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
                    _mod.Attributes.AddAttribute(TweakedAttributeFactory.Get());
                }

                GameSetupManager.Instance.DeleteCache();
            }


        }
    }
}
