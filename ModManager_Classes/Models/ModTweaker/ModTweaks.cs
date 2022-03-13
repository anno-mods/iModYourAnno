using Imya.Models.NotifyPropertyChanged;
using System.Collections.ObjectModel;

namespace Imya.Models.ModTweaker
{
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

        public void Load(Mod mod)
        {
            _mod = mod;

            var files = mod.GetFilesWithExtension("xml").Select(x => Path.GetRelativePath(mod.FullModPath, x)).ToArray();
            var list = new ObservableCollection<TweakerFile>();
            foreach (string filename in files)
            {
                if (TweakerFile.TryInit(mod.FullModPath, filename, out var file))
                {
                    list.Add(file);
                }
            }
            TweakerFiles = list;
        }

        public void Save()
        {
            if (TweakerFiles != null && _mod != null)
            {
                foreach (var f in TweakerFiles)
                {
                    f.Save(_mod.FullModPath);
                }
            }
        }
    }
}
