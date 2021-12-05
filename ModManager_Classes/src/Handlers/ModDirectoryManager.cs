using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModManager_Classes.src.Models;

namespace ModManager_Classes.src.Handlers
{
    public class ModDirectoryManager
    {
        public static ModDirectoryManager Instance { get; set; }
        #region Fields 
        public ObservableCollection<Mod> DisplayedMods { get => _displayedMods; set => _displayedMods = value; }
        public int ActiveMods { get => _activeMods; set => _activeMods = value; }
        public int InactiveMods { get => _inactiveMods; set => _inactiveMods = value; }
        private String ModPath { get; }
        private ObservableCollection<Mod> ModList;
        #endregion

        #region BackgroundFields

        private ObservableCollection<Mod> _displayedMods;
        private int _activeMods = 0;
        private int _inactiveMods = 0;

        #endregion

        #region Constructors 

        public ModDirectoryManager(String ModDirectoryPath)
        {
            Instance = Instance ?? this;
            ModPath = ModDirectoryPath;

            ModList = new ObservableCollection<Mod>();
            LoadModsFromModDirectory();

            DisplayedMods = ModList;
            OnModListChanged();
        }

        #endregion

        #region MemberFunctions

        private void OnModListChanged()
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
            DisplayedMods.OrderBy(x => x.Name).OrderByDescending(x => x.Active);
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
                    .Where(x => x.Name != ".cache")
                    .ToList()
                );
            }
            else
            {
                Console.WriteLine("Mod Directory not found!");
            }
        }

        public bool TrySetModActivationStatus(Mod mod, bool Active)
        {
            //get Mod Path for mod 
            String SourcePath = $"{ModPath}\\{(mod.Active ? "" : "-")}{mod.DirectoryName}\\";
            String TargetPath = $"{ModPath}\\{(Active ? "" : "-")}{mod.DirectoryName}\\";
            try
            {
                if (!TargetPath.Equals(SourcePath))
                {
                    Directory.Move(SourcePath, TargetPath);
                    Console.WriteLine($"{(Active ? "Activated" : "Deactivated")} {mod.Name}. Directory renamed from {SourcePath} to {TargetPath}");
                }                
                mod.Active = Active;
                UpdateModCounts();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to Activate Mod: {mod.Name}. Cause: {e.Message} Source: {SourcePath} Target: {TargetPath}");
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

        /// <summary>
        /// Initializes a Mod from a Mod Folder Root path. If the Name of the folder does not comply with the naming scheme, it renames it to:
        /// "-{ModName}" if inactive.
        /// "{ModName}" if active.
        /// </summary>
        /// <param name="inPath">Path to construct the Mod from.</param>
        /// <returns></returns>
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
            return new Mod(active, Name);
        }


        public delegate bool ModListFilter(Mod m);
        public void FilterMods(ModListFilter filter)
        {
            DisplayedMods = new ObservableCollection<Mod>(ModList.Where(x => filter(x)).ToList());
            OnModListChanged();
        }

        #endregion
    }
}
