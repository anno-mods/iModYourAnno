using Imya.Models.ModMetadata;
using Imya.Models.NotifyPropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Utils
{
    public class ModinfoCreationManager : PropertyChangedNotifier
    {
        public Modinfo ModinfoContext
        {
            get => _modinfoContext;
            set
            {
                _modinfoContext = value;
                OnPropertyChanged(nameof(ModinfoContext));
            }
        }
        private Modinfo _modinfoContext;

        public static ModinfoCreationManager Instance { get; } = new ModinfoCreationManager();

        public ModinfoCreationManager()
        {
            ModinfoContext = new Modinfo();
            ModinfoContext.Version = "5";
        }

        public void Load(String Filename)
        {
            if (ModinfoLoader.TryLoadFromFile(Filename, out var _modinfo))
            {
                ModinfoContext = _modinfo;

                ModinfoContext.IncompatibleIds = ModinfoContext.IncompatibleIds?.Distinct().ToArray();
            }
        }

        public void Save(String Filename)
        {
            ModinfoLoader.TrySaveToFile(Filename, ModinfoContext);
        }

        public void Reset()
        {
            ModinfoContext = new Modinfo();
        }

        public void AddIncompatibleID(String IncompatibleID)
        {
            if (ModinfoContext.IncompatibleIds is not null &&
                ModinfoContext.IncompatibleIds.Any(x => x.Equals(IncompatibleID))) 
                return;

            var List = ModinfoContext.IncompatibleIds?.ToList();
            List?.Add(IncompatibleID);
            ModinfoContext.IncompatibleIds = List?.ToArray();
        }

        public void RemoveIncompatibleID(String IncompatibleID)
        {
            IEnumerable<String>? List = ModinfoContext.IncompatibleIds?.ToList();
            List = List?.Where(x => x.Equals(IncompatibleID));
            ModinfoContext.IncompatibleIds = List?.ToArray();
        }
    }
}
