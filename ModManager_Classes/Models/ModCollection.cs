using Imya.Models.Attributes;
using Imya.Models.NotifyPropertyChanged;
using Imya.Models.Options;
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
            get => _activeMods;
            set => SetProperty(ref _activeMods, value);
        }
        private int _activeMods;

        public int ActiveSizeInMBs
        {
            get => _activeSizeInMBs;
            set => SetProperty(ref _activeSizeInMBs, value);
        }
        private int _activeSizeInMBs = 0;

        public int InstalledSizeInMBs
        {
            get => _installedSizeInMBs;
            set => SetProperty(ref _installedSizeInMBs, value);
        }
        private int _installedSizeInMBs = 0;
        #endregion

        public event ModAddedEventHandler ModAdded = delegate { };
        public delegate void ModAddedEventHandler(Mod m);

        public event UpdatedEventHandler Updated = delegate { };
        public delegate void UpdatedEventHandler();

        public string ModsPath { get; private set; }

        public IReadOnlyList<Mod> Mods => _mods;
        private List<Mod> _mods = new();

        public IEnumerable<String> ModIDs { get => _modids; }
        private List<String> _modids = new();

        private readonly ModCollectionOptions _options;

        public IModComparer ModComparer { get; set; } = new NameComparer();

        /// <summary>
        /// Open mod collection from folder.
        /// </summary>
        /// <param name="path">Path to mods.</param>
        /// <param name="options.Normalize">Remove duplicate "-"</param>
        /// <param name="options.LoadImages">Load image files into memory</param>
        public ModCollection(string path, ModCollectionOptions? options = null)
        {
            ModsPath = path;
            _options = new () {
                Normalize = options?.Normalize ?? false,
                LoadImages = options?.LoadImages ?? false,
            };
        }

        
        /// <summary>
        /// Load all mods.
        /// </summary>
        public async Task LoadModsAsync()
        {
            if (!Directory.Exists(ModsPath))
            {
                _mods = new();
                DisplayedMods = new();
                return;
            }

            _mods = await LoadModsAsync(Directory.EnumerateDirectories(ModsPath)
                .Where(x => !Path.GetFileName(x).StartsWith(".")));

            // TODO option without UI related stuff? having UI classes on top of the model seems better
            DisplayedMods = new ObservableCollection<Mod>(Mods);

            Updated();
        }

        private async Task<List<Mod>> LoadModsAsync(IEnumerable<string> folders)
        {
            var mods = folders.SelectNoNull(x => Mod.TryFromFolder(x)).ToList();
            if (_options.Normalize)
            {
                foreach (var mod in mods)
                    await mod.NormalizeAsync();
            }
            if (_options.LoadImages)
            {
                foreach (var mod in mods)
                {
                    // TODO async and move into class Mod
                    var imagepath = Path.Combine(mod.FullModPath, "banner.png");
                    if (File.Exists(imagepath))
                        mod.InitImageAsFilepath(Path.Combine(imagepath));
                }
            }
            foreach (var mod in mods)
            {
                ModAdded(mod);
            }
            Updated();
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
            // TODO display mods should move to a separate wrapper around ModCollection

            // clear out old mods
            foreach (var mod in _displayedMods)
                mod.StatsChanged -= OnModStatsChanged;

            _displayedMods = new ObservableCollection<Mod>(value.OrderBy(x => x, ModComparer));

            // register for stat changes
            foreach (var mod in _displayedMods)
                mod.StatsChanged += OnModStatsChanged;
            OnModStatsChanged();
            OnPropertyChanged(nameof(DisplayedMods));
        }

        private void OnModStatsChanged()
        {
            var newActive = _mods.Count(x => x.IsActive);
            if (newActive != ActiveMods)
            {
                ActiveMods = newActive;
                ActiveSizeInMBs = (int)Math.Round(_mods.Sum(x => x.IsActive ? x.SizeInMB : 0));
                InstalledSizeInMBs = (int)Math.Round(_mods.Sum(x => x.SizeInMB));
                Console.WriteLine($"{ActiveMods} active mods. {_mods.Count} total found.");

                Updated();
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

            Updated();
        }

        private async Task MoveSingleModIntoAsync(Mod sourceMod, String SourceModsPath, bool AllowOldToOverwrite)
        {
            var (targetMod, targetModPath) = SelectTargetMod(sourceMod);

            if (!AllowOldToOverwrite && !sourceMod.IsUpdateOf(targetMod))
            {
                Console.WriteLine($"Skip update of {sourceMod.FolderName}. Source version: {sourceMod.Modinfo.Version}, target version: {targetMod?.Modinfo.Version}");
                return;
            }

            // do it!
            var status = Directory.Exists(targetModPath) ? ModStatus.Updated : ModStatus.New;
            sourceMod.Attributes.AddAttribute(ModStatusAttributeFactory.Get(status));

            DirectoryEx.CleanMove(Path.Combine(SourceModsPath, sourceMod.FullFolderName), targetModPath);
            Console.WriteLine($"{sourceMod.Attributes.GetByType(AttributeType.ModStatus)}: {sourceMod.FolderName}");

            // mark all duplicate id mods as obsolete
            if (sourceMod.Modinfo.ModID != null)
            {
                var sameModIDs = WhereByModID(sourceMod.Modinfo.ModID).Where(x => x != targetMod);
                foreach (var mod in sameModIDs)
                    await mod.MakeObsoleteAsync(ModsPath);
                // mark mod as updated, since there was the same modid already there
                if (sameModIDs.Any())
                    status = ModStatus.Updated;
            }

            // update mod list, only remove in case of same folder
            if (targetMod is not null)
            {
                _mods.Remove(targetMod);
                targetMod.StatsChanged -= OnModStatsChanged;
            }
            var reparsed = (await LoadModsAsync(new string[] { targetModPath })).First();
            reparsed.Attributes.AddAttribute(ModStatusAttributeFactory.Get(status));
            _mods.Add(reparsed);
            reparsed.StatsChanged += OnModStatsChanged;

            ModAdded(reparsed);
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

        /// <summary>
        /// Permanently delete all mods from collection.
        /// </summary>
        public async Task DeleteAsync(IEnumerable<Mod> mods)
        {
            foreach (var mod in mods)
                await DeleteAsync(mod);

            Updated();
        }

        /// <summary>
        /// Permanently delete mod from collection.
        /// </summary>
        private async Task DeleteAsync(Mod mod)
        {
            await Task.Run(() =>
            {
                try
                {
                    Directory.Delete(mod.FullModPath, true);

                    // remove from the mod lists to prevent access.
                    _mods.Remove(mod);
                    mod.StatsChanged -= OnModStatsChanged;
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
            var activationSet = new HashSet<string>(profile);

            foreach (var mod in Mods)
            {
                bool active = activationSet.Contains(mod.FolderName);
                if (active != mod.IsActive)
                    await mod.ChangeActivationAsync(active);
            }

            Updated();
        }

        public async Task DeactivateAllAsync()
        {
            foreach (Mod mod in Mods)
                await mod.ChangeActivationAsync(false);

            Updated();
        }
        #endregion

        #region ModListFilter

        public delegate bool ModListFilter(Mod m);
        public void FilterMods(ModListFilter filter)
        {
            DisplayedMods = new ObservableCollection<Mod>(Mods.Where(x => filter(x)).ToList());
        }
        #endregion

        public IEnumerable<Mod> WithAttribute(AttributeType attributeType) => Mods.Where(x => x.Attributes.HasAttribute(attributeType));
    }
}
