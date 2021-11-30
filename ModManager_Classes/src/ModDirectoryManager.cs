using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModManager_Classes.src.Models;

namespace ModManager_Classes.src
{
    internal class ModDirectoryManager
    {
        #region Bindables 
        public ObservableCollection<Mod> DisplayedMods { get => _displayedMods; set => _displayedMods = value; }
        public int ActiveMods { get => _activeMods; set => _activeMods = value; }
        public int InactiveMods { get => _inactiveMods; set => _inactiveMods = value; }
        #endregion

        private ObservableCollection<Mod> ModList;
        private ObservableCollection<Mod> _displayedMods;
        private int _activeMods = 0;
        private int _inactiveMods = 0;

        private String ModPath { get; }

        

        #region Constructors 

        public ModDirectoryManager()
        {
            ModList = new ObservableCollection<Mod>();
            DisplayedMods = ModList;

            LoadModsFromModDirectory();
        }

        #endregion

        #region MemberFunctions

        private void ModListChanged()
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
                ModListChanged();
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
            ModListChanged();
        }
                
        #endregion
    }
}
