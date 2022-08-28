using Imya.Models.Attributes;
using Imya.Models.NotifyPropertyChanged;
using Imya.Utils;
using System.Collections;
using System.Collections.Specialized;

namespace Imya.Models
{
    public class ModCollection : PropertyChangedNotifier, IReadOnlyCollection<Mod>, INotifyCollectionChanged
    {
        public static readonly ModCollection Empty = new("");

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

        /// <summary>
        /// Triggers after any change of the collection.
        /// Mods are fully loaded at this point.
        /// </summary>
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        public string ModsPath { get; private set; }

        public IReadOnlyList<Mod> Mods => _mods;
        private List<Mod> _mods = new();

        public IEnumerable<String> ModIDs { get => _modids; }
        private List<String> _modids = new();

        private readonly bool _normalize;
        private readonly bool _loadImages;
        private readonly bool _autofixSubfolder;

        /// <summary>
        /// Open mod collection from folder.
        /// </summary>
        /// <param name="path">Path to mods.</param>
        /// <param name="normalize">Remove duplicate "-"</param>
        /// <param name="loadImages">Load image files into memory</param>
        /// <param name="autofixSubfolder">find data/ in subfolder and move up</param>
        public ModCollection(string path, bool normalize = false, bool loadImages = false, bool autofixSubfolder = false)
        {
            ModsPath = path;
            _normalize = normalize;
            _loadImages = loadImages;
            _autofixSubfolder = autofixSubfolder;
        }

        /// <summary>
        /// Load all mods.
        /// </summary>
        public async Task LoadModsAsync()
        {
            if (Directory.Exists(ModsPath))
            {
                if (_autofixSubfolder)
                    AutofixSubfolders(ModsPath);

                _mods = await LoadModsAsync(Directory.EnumerateDirectories(ModsPath)
                    .Where(x => !Path.GetFileName(x).StartsWith(".")));
                foreach (var mod in _mods)
                    mod.PropertyChanged += Mod_PropertyChanged;
            }
            else
            {
                _mods = new();
            }

            OnActivationChanged(null);
        }

        /// <summary>
        /// If there's no data/ top-level, move all data/ folders up - no matter how deep they are.
        /// Obviously ignore data/ under data/ like in data/abc/data/ situations
        /// </summary>
        private static void AutofixSubfolders(string modsPath)
        {
            foreach (var folder in Directory.EnumerateDirectories(modsPath))
            {
                if (Directory.Exists(Path.Combine(folder, "data")))
                    continue;

                var potentialMods = DirectoryEx.FindFolder(folder, "data").Select(x => Path.GetDirectoryName(x)!);
                foreach (var potentialMod in potentialMods)
                {
                    try
                    {
                        DirectoryEx.CleanMove(potentialMod, Path.Combine(modsPath, Path.GetFileName(potentialMod)));
                    }
                    catch
                    {
                        // TODO should we say something?
                    }
                }

                // only remove the parent folder if all content has been moved
                if (!Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories).Any())
                {
                    try
                    {
                        Directory.Delete(folder, true);
                    }
                    catch
                    {
                        // tough luck, but not harmful
                    }
                }
            }
        }

        private async Task<List<Mod>> LoadModsAsync(IEnumerable<string> folders)
        {
            var mods = folders.SelectNoNull(x => Mod.TryFromFolder(x)).ToList();
            if (_normalize)
            {
                foreach (var mod in mods)
                    await mod.NormalizeAsync();
            }
            if (_loadImages)
            {
                foreach (var mod in mods)
                {
                    // TODO async and move into class Mod
                    var imagepath = Path.Combine(mod.FullModPath, "banner.jpg");
                    if (File.Exists(imagepath))
                        mod.InitImageAsFilepath(Path.Combine(imagepath));
                    else
                    {
                        imagepath = Path.Combine(mod.FullModPath, "banner.png");
                        if (File.Exists(imagepath))
                            mod.InitImageAsFilepath(Path.Combine(imagepath));
                    }
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

        private void Mod_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Mod.IsActive))
            {
                OnActivationChanged(sender as Mod);
            }
        }

        private void OnActivationChanged(Mod? sender)
        {
            // remove mods with IssueModRemoved attribute
            int removedModCount = _mods.Count(x => x.Attributes.HasAttribute(AttributeType.IssueModRemoved));

            int newActiveCount = _mods.Count(x => x.IsActive);
            if (removedModCount > 0 || ActiveMods != newActiveCount)
            {
                ActiveMods = newActiveCount;
                ActiveSizeInMBs = (int)Math.Round(_mods.Sum(x => x.IsActive ? x.SizeInMB : 0));
                InstalledSizeInMBs = (int)Math.Round(_mods.Sum(x => x.SizeInMB));
                Console.WriteLine($"{ActiveMods} active mods. {_mods.Count} total found.");

                // trigger changed events for activation/deactivation
                if (sender is null)
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                else
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, sender, sender));
            }
        }

        #region Add, remove mods
        /// <summary>
        /// Moves mods from collection to mod folder.
        /// Source collection folder will be deleted afterwards.
        /// Existing mods will be overwriten, old names with same mod id deactivated.
        /// </summary>
        public async Task MoveIntoAsync(ModCollection source, bool allowOldToOverwrite = false)
        {
            Directory.CreateDirectory(ModsPath);

            try
            {
                foreach (var sourceMod in source.Mods)
                {
                    await MoveSingleModIntoAsync(sourceMod, source.ModsPath, allowOldToOverwrite);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Move Error: {e.Message}");
            }
            finally
            {
                Directory.Delete(source.ModsPath, true);
            }

            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, Mods.ToList()));
        }

        private async Task MoveSingleModIntoAsync(Mod sourceMod, string sourceModsPath, bool allowOldToOverwrite)
        {
            var (targetMod, targetModPath) = SelectTargetMod(sourceMod);

            if (!allowOldToOverwrite && !sourceMod.IsUpdateOf(targetMod))
            {
                Console.WriteLine($"Skip update of {sourceMod.FolderName}. Source version: {sourceMod.Modinfo.Version}, target version: {targetMod?.Modinfo.Version}");
                return;
            }

            // do it!
            var status = Directory.Exists(targetModPath) ? ModStatus.Updated : ModStatus.New;
            sourceMod.Attributes.AddAttribute(ModStatusAttributeFactory.Get(status));

            DirectoryEx.CleanMove(Path.Combine(sourceModsPath, sourceMod.FullFolderName), targetModPath);
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
                targetMod.PropertyChanged -= Mod_PropertyChanged;
            }
            var reparsed = (await LoadModsAsync(new string[] { targetModPath })).First();
            reparsed.Attributes.AddAttribute(ModStatusAttributeFactory.Get(status));
            _mods.Add(reparsed);
            reparsed.PropertyChanged += Mod_PropertyChanged;

            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new Mod[] { reparsed }));
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
            var deleted = mods.ToList();

            foreach (var mod in mods)
                await DeleteAsync(mod);

            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, deleted));
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
                    mod.PropertyChanged -= Mod_PropertyChanged;
                }
                catch (DirectoryNotFoundException)
                {
                    // remove from the mod lists to prevent access.
                    _mods.Remove(mod);
                    mod.PropertyChanged -= Mod_PropertyChanged;
                }
                catch (Exception e)
                {
                    mod.Attributes.Add(new GenericAttribute() { AttributeType = AttributeType.IssueModAccess, Description = new SimpleText("Could not delete this mod.") });
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

            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, Mods.ToList()));
        }
        #endregion

        public IEnumerable<Mod> WithAttribute(AttributeType attributeType) => Mods.Where(x => x.Attributes.HasAttribute(attributeType));

        #region IReadOnlyCollection
        public int Count => _mods.Count;
        public IEnumerator<Mod> GetEnumerator() => _mods.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _mods.GetEnumerator();
        #endregion
    }
}
