using Imya.Models.ModTweaker;
using Imya.Models.NotifyPropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Utils
{
    public class ModTweakingManager : PropertyChangedNotifier
    {
        public ObservableCollection<TweakerFile> TweakerFiles
        {
            get => _tweakerFiles;
            set
            {
                _tweakerFiles = value;
                OnPropertyChanged(nameof(TweakerFiles));
            }
        }
        private ObservableCollection<TweakerFile> _tweakerFiles;

        public static ModTweakingManager Instance;

        public ModTweakingManager()
        {
            TweakerFiles = new ObservableCollection<TweakerFile>();
            Instance ??= this;
        }

        public void RegisterFiles(IEnumerable<String> files)
        {
            foreach (String file in files)
            {
                AddTweakerFile(file);
            }
        }

        private void AddTweakerFile(String filename)
        {
            if (TweakerFile.TryInit(filename, out var file))
            {
                TweakerFiles.Add(file);
            }
        }

        public void Save()
        {
            foreach (var f in TweakerFiles)
            {
                f.Save();
            }
        }

        public void Clear()
        {
            TweakerFiles.Clear();
        }

        public bool HasElements()
        { 
            return TweakerFiles.Any();
        }
    }
}
