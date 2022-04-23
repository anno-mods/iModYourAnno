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

        public String IncompatibleIDsJoined 
        {
            get => _incompatible_ids_joined;
            set
            {
                _incompatible_ids_joined = value;
                OnPropertyChanged(nameof(IncompatibleIDsJoined));
            }
        }
        private String _incompatible_ids_joined;

        public String ModDepsJoined
        {
            get => _mod_deps_joined;
            set
            {
                _mod_deps_joined = value;
                OnPropertyChanged(nameof(ModDepsJoined));
            }
        }
        private String _mod_deps_joined;

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
                //
                IncompatibleIDsJoined = StringArrToString(ModinfoContext?.IncompatibleIds) ?? "";
                ModDepsJoined = StringArrToString(ModinfoContext?.ModDependencies) ?? "";
            }
        }

        public void Save(String Filename)
        {
            ModinfoContext.IncompatibleIds = StringToStringArr(IncompatibleIDsJoined);
            ModinfoContext.ModDependencies = StringToStringArr(ModDepsJoined);
            ModinfoLoader.TrySaveToFile(Filename, ModinfoContext);
        }

        public void Reset()
        {
            ModinfoContext = new Modinfo();
        }

        private String[]? StringToStringArr(String? _string)
        {
            if (_string is null) return null;
            var split =  _string.Split(";");

            if (split.Length == 1 && split[0].Equals(String.Empty)) return null;

            return split;
        }

        private String? StringArrToString(String[]? _string)
        {
            if (_string is null) return null;
            return String.Join(";", _string);
        }
    }
}
