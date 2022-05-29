using Imya.Models.ModMetadata;
using Imya.Models.NotifyPropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Utils
{
    public class ModinfoFactory : PropertyChangedNotifier
    {
        public readonly char ArraySplitter = ';';
        public Modinfo ModinfoContext
        {
            get => _modinfoContext;
            private set
            {
                _modinfoContext = value;

                IncompatibleIDsJoined = StringArrToString(_modinfoContext?.IncompatibleIds) ?? "";
                ModDepsJoined = StringArrToString(_modinfoContext?.ModDependencies) ?? "";

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
                ModinfoContext.IncompatibleIds = StringToStringArr(_incompatible_ids_joined);
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
                ModinfoContext.ModDependencies = StringToStringArr(_mod_deps_joined);
                OnPropertyChanged(nameof(ModDepsJoined));
            }
        }
        private String _mod_deps_joined;

        //* this should not exist!!!


        public ModinfoFactory()
        {
            ModinfoContext = new Modinfo();
        }

        public ModinfoFactory(Modinfo _)
        {
            ModinfoContext = _;
        }

        public void Reset()
        {
            ModinfoContext = new Modinfo();
        }

        public Modinfo? GetResult()
        {
            return ModinfoContext;
        }

        public void SetModDependencies(String ArrayString)
        {
            ModinfoContext.ModDependencies = StringToStringArr(ArrayString);
        }

        public void SetIncompatibleIDs(String ArrayString)
        {
            ModinfoContext.IncompatibleIds = StringToStringArr(ArrayString);
        }

        private String[]? StringToStringArr(String? _string)
        {
            if (_string is null) return null;
            var split =  _string.Split(ArraySplitter);

            if (split.Length == 1 && split[0].Equals(String.Empty)) return null;

            return split;
        }

        private String? StringArrToString(String[]? _string)
        {
            if (_string is null) return null;
            return String.Join(ArraySplitter, _string);
        }
    }
}
