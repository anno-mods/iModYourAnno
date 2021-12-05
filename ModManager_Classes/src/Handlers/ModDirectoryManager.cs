using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModManager_Classes.src.Models;

namespace ModManager_Classes.src.Handlers
{
    internal class ModDirectoryManager
    {
        #region Fields 
        public ObservableCollection<Mod> DisplayedMods { get => _displayedMods; set => _displayedMods = value; }
        public int ActiveMods { get => _activeMods; set => _activeMods = value; }
        public int InactiveMods { get => _inactiveMods; set => _inactiveMods = value; }
        private String ModPath { get; }
        #endregion

        #region Properties

        private ObservableCollection<Mod> ModList;

        #endregion

        #region BackgroundFields

        private ObservableCollection<Mod> _displayedMods;
        private int _activeMods = 0;
        private int _inactiveMods = 0;

        #endregion

        #region Constructors 

        public ModDirectoryManager()
        {
            ModPath = Properties.Resources.MOD_DIRECTORY_PATH;

            ModList = new ObservableCollection<Mod>();
            DisplayedMods = ModList;

            LoadModsFromModDirectory();
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
        }

        public void OrderDisplayed()
        {
            DisplayedMods.OrderBy(x => x.Name).OrderByDescending(x => x.Active);
        }

        public void LoadModsFromModDirectory()
        {
            if (Directory.Exists(ModPath))
            {
                DisplayedMods = new ObservableCollection<Mod>(
                    Directory.EnumerateDirectories(ModPath)
                    .Select(
                        x => new Mod(Path.GetFileName(x))
                    )
                    .Where(x => x.Name != ".cache")
                    .ToList()
                );
                OnModListChanged();
            }
            else
            {
                Console.WriteLine("Mod Directory not found!");
            }
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
