using Imya.Enums;
using Imya.Models.ModMetadata;
using Imya.Models.NotifyPropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public ObservableCollection<DlcId> DLCs { get; set; } = new();

        public ModinfoFactory()
        {
            ModinfoContext = new Modinfo();
        }

        public ModinfoFactory(Modinfo _)
        {
            ModinfoContext = _;
            LoadModinfo(ModinfoContext);
        }

        public void Reset()
        {
            ModinfoContext = new Modinfo();
            LoadModinfo(ModinfoContext);
        }

        public void LoadModinfo(Modinfo _modinfo)
        {
            IncompatibleIDsJoined = StringArrToString(ModinfoContext?.IncompatibleIds) ?? "";
            ModDepsJoined = StringArrToString(ModinfoContext?.ModDependencies) ?? "";

            DLCs.Clear();
            if (ModinfoContext?.DLCDependencies is not null)
            {
                foreach (Dlc _dlc in ModinfoContext?.DLCDependencies!)
                {
                    if (_dlc.DLC is null) continue;
                    DLCs.Add((DlcId)_dlc.DLC);
                }
            }
        }

        public IEnumerable<DlcId> GetRemainingDlcIds()
        {
            Type DlcType = typeof(DlcId);
            Array ids = DlcType.GetEnumValues();
            foreach (object x in ids)
            {
                DlcId id = (DlcId)x;
                if (!DLCs.Contains(id)) yield return id;
            }
        }


        public Modinfo? GetResult()
        {
            ModinfoContext.IncompatibleIds = StringToStringArr(_incompatible_ids_joined);
            ModinfoContext.ModDependencies = StringToStringArr(_mod_deps_joined);
            ModinfoContext.DLCDependencies = DLCs.Count > 0 ? DLCs.Select(_id => new Dlc { DLC = _id, Dependant = DlcRequirement.required }).ToArray() : null;

            return ModinfoContext;
        }

        public void AddDLC(DlcId _dlc)
        {
            DLCs.Add( _dlc );
        }

        public void RemoveDLC(DlcId _dlc)
        {
            DLCs.Remove(_dlc);
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
