using Imya.Models.NotifyPropertyChanged;
using Imya.Utils;
using System.Collections.ObjectModel;

namespace Imya.Models
{
    public class ModCollection : PropertyChangedNotifier
    {
        #region global active collection
        public static ModCollection? Global
        {
            get => _active;
            set
            {
                if (_active == value) return;
                if (_active is not null)
                {
                    GameSetupManager.Instance.GameRootPathChanged -= _active.OnModPathChanged;
                    GameSetupManager.Instance.ModDirectoryNameChanged -= _active.OnModPathChanged;
                }
                if (value is not null)
                {
                    GameSetupManager.Instance.GameRootPathChanged += value.OnModPathChanged;
                    GameSetupManager.Instance.ModDirectoryNameChanged += value.OnModPathChanged;
                }
                _active = value;
            }
        }
        private static ModCollection? _active;
        #endregion

        #region UI related
        public ObservableCollection<Mod> DisplayedMods
        {
            get => _displayedMods;
            set => SetDisplayMods(value);
        }
        private ObservableCollection<Mod> _displayedMods = new();

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
        private int _activeMods;

        public int InactiveMods
        {
            get
            {
                return _inactiveMods;
            }
            set
            {
                _inactiveMods = value;
                OnPropertyChanged(nameof(InactiveMods));
            }
        }
        private int _inactiveMods;
        #endregion

        public string ModsPath { get; private set; }
        public List<Mod> Mods { get; private set; } = new();

        public struct Options
        {
            public bool Normalize { get; init; }
            public bool LoadImages { get; init; }
        }
        private readonly Options _options;

        /// <summary>
        /// Open mod collection from folder.
        /// </summary>
        /// <param name="path">Path to mods.</param>
        /// <param name="options.Normalize">Remove duplicate "-"</param>
        /// <param name="options.LoadImages">Load image files into memory</param>
        public ModCollection(string path, Options? options = null)
        {
            ModsPath = path;
            _options = new () {
                Normalize = options?.Normalize ?? false,
                LoadImages = options?.LoadImages ?? false
            };
        }
        
        /// <summary>
        /// Load all mods.
        /// </summary>
        public async Task LoadModsAsync()
        {
            Mods = await LoadModsAsync(Directory.EnumerateDirectories(ModsPath)
                .Where(x => !Path.GetFileName(x).StartsWith(".")));

            // TODO option without UI related stuff? having UI classes on top of the model seems better
            DisplayedMods = new ObservableCollection<Mod>(Mods);
        }

        private async Task<List<Mod>> LoadModsAsync(IEnumerable<string> folders)
        {
            var mods = folders.SelectNoNull(x => Mod.TryFromFolder(x)).ToList();
            if (_options.Normalize)
            {
                foreach (var mod in Mods)
                    await mod.NormalizeAsync();
            }
            if (_options.LoadImages)
            {
                foreach (var mod in Mods)
                {
                    // TODO async and move into class Mod
                    var imagepath = Path.Combine(mod.FullModPath, "banner.png");
                    if (File.Exists(imagepath))
                        mod.InitImageAsFilepath(Path.Combine(imagepath));
                }
            }
            return mods;
        }

        #region MemberFunctions
        private async void OnModPathChanged(string _)
        {
            ModsPath = GameSetupManager.Instance.GetModDirectory();

            await LoadModsAsync();
        }

        private void SetDisplayMods(ObservableCollection<Mod> value)
        {
            _displayedMods = new ObservableCollection<Mod>(value.
                OrderBy(x => x.Name.Text).
                OrderBy(x => x.Category.Text).
                OrderByDescending(x => x.IsActive).
                ToList());
            UpdateModCounts();
            OnPropertyChanged(nameof(DisplayedMods));
        }

        private void UpdateModCounts()
        {
            var newActive = Mods.Count(x => x.IsActive);
            var newInactive = Mods.Count - newActive;
            if (newActive != ActiveMods || newInactive != InactiveMods)
            {
                ActiveMods = newActive;
                InactiveMods = newInactive;
                Console.WriteLine($"Found: {Mods.Count}, Active: {ActiveMods}, Inactive: {InactiveMods}");
            }
        }

        #region Add, remove mods
        /// <summary>
        /// Moves mods from collection to mod folder.
        /// Source collection folder will be deleted afterwards.
        /// Existing mods will be overwriten, old names with same mod id deactivated.
        /// </summary>
        public async Task AddAsync(ModCollection source)
        {
            // TODO status should be handled outside of this function. it unnecessarily drives complexity here.

            // TODO issue handling
            // TODO mods without modid / modinfo.json
            foreach (var sourceMod in source.Mods)
            {
                var targetMod = FirstByFolderName(sourceMod.FolderName);
                string targetModPath = Path.Combine(ModsPath, targetMod?.FullFolderName ?? sourceMod.FullFolderName);
                sourceMod.Status = Directory.Exists(targetModPath) ? ModStatus.Updated : ModStatus.New;
                DirectoryEx.CleanMove(Path.Combine(source.ModsPath, sourceMod.FullFolderName), targetModPath);

                // mark all duplicate id mods as obsolete
                if (sourceMod.Modinfo.ModID != null)
                {
                    var sameModIDs = WhereByModID(sourceMod.Modinfo.ModID).Where(x => x != targetMod);
                    foreach (var mod in sameModIDs)
                        await mod.MakeObsoleteAsync(ModsPath);
                    // mark mod as updated, since there was the same modid already there
                    if (sameModIDs.Any())
                        sourceMod.Status = ModStatus.Updated;
                }

                // only remove in case of same folder
                if (targetMod is not null)
                    Mods.Remove(targetMod);

                var reparsed = (await LoadModsAsync(new string[] { targetModPath })).First();
                reparsed.Status = sourceMod.Status;
                Mods.Add(reparsed);
            }
            // TODO individual load is faster, but optimization can be done later
            // await LoadModsAsync();
            DisplayedMods = new ObservableCollection<Mod>(Mods);
        }

        private Mod? FirstByFolderName(string folderName, bool ignoreActivation = true)
        {
            var match = Mods.Where(x => (ignoreActivation ? x.FolderName : x.FullFolderName) == folderName).ToArray();

            // prefer activated one in case of two
            if (ignoreActivation && match.Length == 2)
                return match[0].IsActive ? match[0] : match[1];

            return match.FirstOrDefault();
        }

        private IEnumerable<Mod> WhereByModID(string modID)
        {
            return Mods.Where(x => x.Modinfo.ModID == modID);
        }

        // TODO
        //public void Delete(Mod mod)
        //{
        //    try
        //    {
        //        //delete the directory
        //        Directory.Delete(GetModPath(mod));

        //        //remove from the mod lists to prevent access.
        //        Mods.Remove(mod);
        //        DisplayedMods.Remove(mod);
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine($"Failed to delete Mod: {mod.Category} {mod.Name}. Cause: {e.Message}");
        //    }
        //}
        #endregion

        public void LoadProfile(ModActivationProfile profile)
        {
            FilterMods(x => profile.ContainsID(x.FolderName));
        }

        #endregion

        #region ModListFilter

        public delegate bool ModListFilter(Mod m);
        public void FilterMods(ModListFilter filter)
        {
            DisplayedMods = new ObservableCollection<Mod>(Mods.Where(x => filter(x)).ToList());
        }
        #endregion
    }
}
