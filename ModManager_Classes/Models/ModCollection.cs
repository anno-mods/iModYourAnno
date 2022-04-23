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
            if (!Directory.Exists(ModsPath))
            {
                Mods = new();
                DisplayedMods = new();
                return;
            }

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

        /// <summary>
        /// </summary>
        /// <param name="m"></param>
        /// <returns>index of <paramref name="m"/> in the DisplayedMods property</returns>
        public int IndexOf(Mod m)
        { 
            if(DisplayedMods.Contains(m)) return DisplayedMods.IndexOf(m);

            return -1;
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
        public async Task MoveIntoAsync(ModCollection source, bool AllowOldToOverwrite = false)
        {
            Directory.CreateDirectory(ModsPath);

            // TODO status should be handled outside of this function. it unnecessarily drives complexity here.
            // TODO external issue handling
            // done, boss! :kekw:
            try
            {
                foreach (var sourceMod in source.Mods)
                {
                    await MoveSingleModIntoAsync(sourceMod, source.ModsPath, AllowOldToOverwrite);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Move Error: {e.Message}");
            }
            finally
            {
                Directory.Delete(source.ModsPath, true);
                DisplayedMods = new ObservableCollection<Mod>(Mods);
            }
        }

        public async Task MoveSingleModIntoAsync(Mod sourceMod, String SourceModsPath, bool AllowOldToOverwrite)
        {
            var (targetMod, targetModPath) = SelectTargetMod(sourceMod);

            if (!AllowOldToOverwrite && IsSourceOutdated(sourceMod, targetMod))
            {
                Console.WriteLine($"Skip update of {sourceMod.FolderName}. Source version: {sourceMod.Modinfo.Version}, target version: {targetMod?.Modinfo.Version}");
                return;
            }

            // do it!
            sourceMod.Status = Directory.Exists(targetModPath) ? ModStatus.Updated : ModStatus.New;
            DirectoryEx.CleanMove(Path.Combine(SourceModsPath, sourceMod.FullFolderName), targetModPath);
            Console.WriteLine($"{sourceMod.Status}: {sourceMod.FolderName}");

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

            // update mod list, only remove in case of same folder
            if (targetMod is not null)
                Mods.Remove(targetMod);
            var reparsed = (await LoadModsAsync(new string[] { targetModPath })).First();
            reparsed.Status = sourceMod.Status;
            Mods.Add(reparsed);
        }

        private (Mod?, string) SelectTargetMod(Mod sourceMod)
        {
            // select target mod
            var targetMod = FirstByFolderName(sourceMod.FolderName);
            string targetModPath = Path.Combine(ModsPath, targetMod?.FullFolderName ?? sourceMod.FullFolderName);

            // re-select target mod when modids are different (safeguard after 9 tries)
            var iteration = 1;
            while (iteration < 10 &&
                sourceMod.Modinfo.ModID is not null &&
                targetMod?.Modinfo.ModID is not null &&
                sourceMod.Modinfo.ModID != targetMod.Modinfo.ModID)
            {
                targetMod = FirstByFolderName($"{sourceMod.FolderName}-{iteration}");
                targetModPath = Path.Combine(ModsPath, targetMod?.FullFolderName ?? $"{sourceMod.FullFolderName}-{iteration}");
                iteration++;
            }

            return (targetMod, targetModPath);
        }

        private static bool IsSourceOutdated(Mod sourceMod, Mod? targetMod)
        {
            if (targetMod is null || targetMod.Modinfo.Version is null)
                return false;

            if (sourceMod.Modinfo.Version is null && targetMod.Modinfo.Version is not null)
                return true;

            if (!VersionEx.TryParse(targetMod.Modinfo.Version, out var targetVersion))
                return false;

            if (!VersionEx.TryParse(sourceMod.Modinfo.Version, out var sourceVersion))
                return true;

            return sourceVersion < targetVersion;
        }

        /// <summary>
        /// Permanently delete all mods from collection.
        /// </summary>
        public async Task DeleteAsync(IEnumerable<Mod> mods)
        {
            foreach (var mod in mods)
                await DeleteAsync(mod);
        }

        /// <summary>
        /// Permanently delete mod from collection.
        /// </summary>
        public async Task DeleteAsync(Mod mod)
        {
            await Task.Run(() =>
            {
                try
                {
                    Directory.Delete(mod.FullModPath, true);

                    // remove from the mod lists to prevent access.
                    Mods.Remove(mod);
                    // remove on DisplayMods cannot be than from non-Dispatcher threads
                    DisplayedMods = new ObservableCollection<Mod>(Mods);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to delete Mod: {mod.Category} {mod.Name}. Cause: {e.Message}");
                }
            });
        }
        #endregion

        /// <summary>
        /// Returns mod with given folder name. Activated mods come first.
        /// </summary>
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

        public async Task LoadProfileAsync(ModActivationProfile profile)
        {
            await DeactivateAllAsync();
            
            foreach (String ModDirectoryName in profile)
            {
                Mod? mod = Mods.FirstOrDefault(x => x.FolderName.Equals(ModDirectoryName));

                if (mod is not null)
                    await mod.ChangeActivationAsync(true);
            }
        }

        public async Task DeactivateAllAsync()
        {
            foreach (Mod mod in Mods)
                await mod.ChangeActivationAsync(false);
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
