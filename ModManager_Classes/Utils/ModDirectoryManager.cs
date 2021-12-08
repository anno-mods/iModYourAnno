using Imya.Models;
using Imya.Models.ModMetadata;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Imya.Utils
{
    public class ModDirectoryManager : INotifyPropertyChanged
    {
        public static ModDirectoryManager Instance { get; private set; }

        #region Fields 

        public ObservableCollection<Mod> DisplayedMods
        {
            get => _displayedMods;
            set
            {
                _displayedMods = value;
                OnDisplayedModListChanged();
                OnPropertyChanged("DisplayedMods");
            }
        }

        public int ActiveMods 
        { 
            get
            { 
                return _activeMods; 
            }
            set 
            { 
                _activeMods = value;
                OnPropertyChanged("ActiveMods");
            } 
        }
        public int InactiveMods 
        { 
            get
            { 
                return _inactiveMods; 
            }
            set 
            { 
                _inactiveMods = value;
                OnPropertyChanged("InactiveMods");
            } 
        }

        private String ModPath { get; }
        private ObservableCollection<Mod> ModList;
        #endregion

        #region BackgroundFields

        private ObservableCollection<Mod> _displayedMods;
        private int _activeMods;
        private int _inactiveMods;

        #endregion

        #region Constructors 

        public ModDirectoryManager(String ModDirectoryPath)
        {
            Instance = Instance ?? this;
            ModPath = ModDirectoryPath;

            ModList = new ObservableCollection<Mod>();
            LoadModsFromModDirectory();

            DisplayedMods = ModList;
        }

        #endregion

        #region MemberFunctions

        private void OnDisplayedModListChanged()
        {
            UpdateModCounts();
            OrderDisplayed();
        }

        private void UpdateModCounts()
        {
            ActiveMods = ModList.Count(x => x.Active);
            InactiveMods = ModList.Count(x => !x.Active);
            Console.WriteLine($"Found: {ModList.Count}, Active: {ActiveMods}, Inactive: {InactiveMods}");
        }

        public void OrderDisplayed()
        {
            _displayedMods = new ObservableCollection<Mod>(DisplayedMods.OrderBy(x => x.Name.Text).OrderBy(x => x.Category.Text).OrderByDescending(x => x.Active).ToList());
        }

        public void LoadModsFromModDirectory()
        {
            if (Directory.Exists(ModPath))
            {
                ModList = new ObservableCollection<Mod>(
                    Directory.EnumerateDirectories(ModPath)
                    .Select(
                        x => InitMod(x)
                    )
                    .Where(x => x.DirectoryName != ".cache")
                    .ToList()
                );
            }
            else
            {
                Console.WriteLine("Mod Directory not found!");
            }
        }

        public bool Activate(Mod mod)
        {
            return TrySetModActivationStatus(mod, true);
        }

        public bool Deactivate(Mod mod)
        {
            return TrySetModActivationStatus(mod, false);
        }

        public bool TrySetModActivationStatus(Mod mod, bool Active)
        {
            //get Mod Path for mod 
            String SourcePath = Path.Combine($"{ModPath}", $"{(mod.Active ? "" : "-")}{mod.DirectoryName}", "");
            String TargetPath = Path.Combine($"{ModPath}", $"{(Active ? "" : "-")}{mod.DirectoryName}", "");
            try
            {
                if (mod.Active != Active)
                {
                    Directory.Move(SourcePath, TargetPath);
                    Console.WriteLine($"{(Active ? "Activated" : "Deactivated")} {mod.Category} {mod.Name}. Directory renamed from {SourcePath} to {TargetPath}");
                }                
                mod.Active = Active;
                UpdateModCounts();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to Activate Mod: {mod.Category} {mod.Name}. Cause: {e.Message} Source: {SourcePath} Target: {TargetPath}");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if a directory name starts with any '-' chars.
        /// </summary>
        /// <param name="s">input name</param>
        /// <param name="result">out trimmed name</param>
        /// <returns>true, if there is no dash at the start (mod is active), false, if there is one.</returns>
        private bool TryTrimDash(String s, out String result)
        {
            if (!s.StartsWith('-'))
            {
                result = s;
                return true;
            }
            while (s.StartsWith('-'))
            {
                s = s.Substring(1);
            }
            result = s;
            return false;
        }

        public bool TrySerializeMetadata(String MetadataFile, out Modinfo? metadata)
        {
            try
            {
                metadata = JsonConvert.DeserializeObject<Modinfo>(File.ReadAllText(MetadataFile));
                return true;
            }
            catch (JsonSerializationException e)
            {
                metadata = null;
                Console.WriteLine("Json Serialization failed: {0}", MetadataFile);
            }
            catch (IOException e)
            {
                metadata = null;
                Console.WriteLine("File not found: {0}", MetadataFile);
            }
            return false;
        }

        /// <summary>
        /// Initializes a Mod from a Mod Folder Root path. If the Name of the folder does not comply with the naming scheme, it renames it to:
        /// "-{ModName}" if inactive.
        /// "{ModName}" if active.
        /// </summary>
        /// <param name="inPath">Path to construct the Mod from.</param>
        /// <returns>the constructed mod</returns>
        private Mod InitMod(String inPath)
        {
            bool active = TryTrimDash(Path.GetFileName(inPath), out String Name);
            String TargetPath = $"{ModPath}\\{(active ? "" : "-")}{Name}";
            if (!inPath.Equals(TargetPath))
            {
                try
                {
                    Directory.Move(inPath, TargetPath);
                }
                catch (IOException e)
                {
                    Console.WriteLine($"Mod Load error: Could not move Directory {inPath} because it is being used by another process.");
                    Console.WriteLine(e.Message);
                }
            }
            TrySerializeMetadata(Path.Combine(TargetPath, "modinfo.json"), out var metadata);
            return new Mod(active, Name, metadata);
        }
        #endregion

        #region ModListFilter

        public delegate bool ModListFilter(Mod m);
        public void FilterMods(ModListFilter filter)
        {
            DisplayedMods = new ObservableCollection<Mod>(ModList.Where(x => filter(x)).ToList());
        }
        #endregion

        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler? PropertyChanged = delegate { };
        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler is PropertyChangedEventHandler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
