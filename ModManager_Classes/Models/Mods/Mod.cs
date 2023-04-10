using Imya.Models.ModMetadata;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Imya.Utils;
using Imya.Models.NotifyPropertyChanged;
using Imya.Models.Attributes;

namespace Imya.Models.Mods
{
    public class Mod : PropertyChangedNotifier
    {
        #region ModLoader info
        /// <summary>
        /// Folder name including activation "-".
        /// </summary>
        public string FullFolderName => (IsActive ? "" : "-") + FolderName;

        /// <summary>
        /// Folder name excluding activation "-".
        /// </summary>
        public string FolderName { get; private set; }

        /// <summary>
        /// "-" activation.
        /// </summary>
        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }
        private bool _isActive;

        /// <summary>
        /// "-" activation and valid.
        /// </summary>
        public bool IsActiveAndValid => _isActive && !IsRemoved;

        /// <summary>
        /// Mod disappeared.
        /// </summary>
        public bool IsRemoved
        {
            get => Attributes.HasAttribute(AttributeType.IssueModRemoved);
            set
            {
                if (IsRemoved == value)
                    return;
                OnPropertyChanged(nameof(IsRemoved));
            }
        }

        public bool IsObsolete
        {
            get => _isObsolete;
            set
            {
                if (_isObsolete == value)
                    return;
                _isObsolete = value;
                OnPropertyChanged(nameof(IsObsolete));
            }
        }
        private bool _isObsolete; 

        /// <summary>
        /// Full path to mod folder.
        /// </summary>
        public string FullModPath => Path.Combine(BasePath, FullFolderName);
        public string BasePath { get; private set; } // TODO use ModDirectory as parent and retrieve it from there as soon as it's not a global manager anymore
        #endregion

        #region Mandatory Mod Manager info (with defaults)
        /// <summary>
        /// Name without category.
        /// </summary>
        public IText Name => Modinfo.ModName;

        /// <summary>
        /// Category with default "NoCategory".
        /// </summary>
        public IText Category => Modinfo.Category;

        public string ModID => Modinfo.ModID ?? FolderName;
        #endregion

        #region Optional Mod Manager info
        public LocalizedModinfo Modinfo { get; private init; }
        public ImyaImageSource? Image { get; private set; }

        public Version? Version { get; private init; }

        public bool HasVersion { get => Modinfo.Version is not null; }
        public bool HasDescription { get => Modinfo.Description is not null; }
        public bool HasKnownIssues { get => Modinfo.KnownIssues is not null && Modinfo.KnownIssues.Length > 0; }
        public bool HasDlcDependencies { get => Modinfo.DLCDependencies is not null && Modinfo.DLCDependencies.Length > 0; }
        public bool HasCreator { get => Modinfo.CreatorName is not null; }
        public bool HasImage { get => Image is not null; }

        public bool HasModID { get => Modinfo.ModID is not null; }
        #endregion

        public float SizeInMB { get; private set; }

        public Attributes.AttributeCollection Attributes { get; } = new();

        #region loading
        public static Mod? TryFromFolder(string modFolderPath)
        {
            var basePath = Path.GetDirectoryName(modFolderPath);
            if (basePath is null || !Directory.Exists(modFolderPath)) return null;

            ModinfoLoader.TryLoadFromFile(Path.Combine(modFolderPath, "modinfo.json"), out var modinfo);
            return new Mod(Path.GetFileName(modFolderPath), modinfo, basePath);
        }

        /// <param name="folderName">i.e. "[Gameplay] AI Shipyard"</param>
        /// <param name="basePath">absolute path without folderName</param>
        public Mod(string folderName, Modinfo? modinfo, string basePath)
        {
            IsActive = !folderName.StartsWith("-");
            FolderName = IsActive ? folderName : folderName[1..];
            BasePath = basePath;

            // create metadata if needed
            if (modinfo is null)
            {
                modinfo = new();
                Attributes.AddAttribute(MissingModinfoAttributeFactory.Get());
            }

            if (modinfo.ModName is null || !modinfo.ModName.HasAny())
            {
                bool matches = MatchNameCategory(FolderName, out var _category, out var _name);
                modinfo.ModName = new FakeLocalized(matches ? _name : FolderName);
                if (matches) modinfo.Category = new FakeLocalized(_category);
            }

            Modinfo = modinfo.GetLocalized(FolderName);

            // Just construct as base64 for now. 
            // TODO move to separate async function
            if (Modinfo.Image is not null)
            {
                Image = new ImyaImageSource();
                Image.ConstructAsBase64Image(Modinfo.Image);
            }

            if (VersionEx.TryParse(Modinfo.Version, out var version))
                Version = version;

            // Just get the size
            // TODO move to separate async?
            var info = new DirectoryInfo(FullModPath);
            SizeInMB = (float)Math.Round((decimal)info.EnumerateFiles("*", SearchOption.AllDirectories).Sum(x => x.Length) / 1024 / 1024, 1);
        }

        public void InitImageAsFilepath(string ImagePath)
        {
            Image = new ImyaImageSource();
            Image.ConstructAsFilepathImage(ImagePath);
        }
        #endregion

        #region modifying actions
        /// <summary>
        /// Remove duplicate "-" from folder name.
        /// Will overwrite any existing folder.
        /// Note: Modinfo will not be updated.
        /// </summary>
        /// <returns></returns>
        public async Task NormalizeAsync()
        {
            if (!FolderName.StartsWith("-")) return;

            var trimAllDash = FolderName;
            while (trimAllDash.StartsWith("-"))
                trimAllDash = trimAllDash[1..];

            await Task.Run(() =>
            {
                string sourcePath = Path.Combine(BasePath, FullFolderName);
                string targetPath = Path.Combine(BasePath, IsActive ? "" : "-" + trimAllDash);
                try
                {
                    DirectoryEx.CleanMove(sourcePath, targetPath);
                    Console.WriteLine($"Removed duplicate '-' from {FullFolderName}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to remove duplicate '-' from {FullFolderName}. Cause: {e.Message}");
                }
            });

            FolderName = trimAllDash;
        }

        /// <summary>
        /// Change activation by renaming the mod folder.
        /// Note: target folder will be overwritten if both active and inactive states are available.
        /// </summary>
        internal void AdaptToActiveStatus(bool active)
        {
            string sourcePath = Path.Combine(BasePath, FullFolderName);
            string targetPath = Path.Combine(BasePath, (active ? "" : "-") + FolderName);

            try
            {
                DirectoryEx.CleanMove(sourcePath, targetPath);
                IsActive = active;
            }
            catch (DirectoryNotFoundException)
            {
                IsRemoved = true;
                throw new InvalidOperationException($"Failed to access mod: {FolderName}. Cause: The mod has been removed");
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Failed to access mod: {FolderName}. Cause: {e.Message}");
            }
        }

        /// <summary>
        /// Deactivate and give it a special flag as deletion candidate.
        /// </summary>
        #endregion

        #region readonly actions
        /// <summary>
        /// Check if mod name, category and creator fields contain all keywords.
        /// </summary>
        public bool HasKeywords(string spaceSeparatedKeywords)
        {
            var keywords = spaceSeparatedKeywords.Split(" ");
            return keywords.Aggregate(true, (isMatch, keyword) => isMatch &= HasKeyword(keyword));
        }

        private bool HasKeyword(string keyword)
        {
            var k = keyword.ToLower();
            return Modinfo.ModName.Text.ToLower().Contains(k) ||
                (Modinfo.Category?.Text.ToLower().Contains(k) ?? false) ||
                (Modinfo.CreatorName?.ToLower().Contains(k) ?? false);
        }

        /// <summary>
        /// Return all files with a specific extension.
        /// </summary>
        public IEnumerable<string> GetFilesWithExtension(string extension)
        {
            return Directory.EnumerateFiles(FullModPath, $"*.{extension}", SearchOption.AllDirectories);
        }

        private static bool MatchNameCategory(string folderName, out string category, out string name)
        {
            string CategoryPattern = @"[[][a-z]+[]]";
            category = Regex.Match(folderName, CategoryPattern, RegexOptions.IgnoreCase).Value.TrimStart('[').TrimEnd(']');

            string NamePattern = @"[^]]*";
            name = Regex.Match(folderName, NamePattern, RegexOptions.RightToLeft).Value.TrimStart(' ');

            return !name.Equals("") && !category.Equals("");
        }
        #endregion

        #region comparisons
        /// <summary>
        /// Check if mod has the same version and content as the target mod.
        /// </summary>
        public bool HasSameContentAs(Mod? target)
        {
            if (target is null) return false;

            if (Modinfo.Version != target.Modinfo.Version)
                return false;

            var dirA = new DirectoryInfo(FullModPath);
            var dirB = new DirectoryInfo(target.FullModPath);
            var listA = dirA.GetFiles("*", SearchOption.AllDirectories);
            var listB = dirB.GetFiles("*", SearchOption.AllDirectories);

            // TODO filter tweaking files!

            // first compare path only to avoid costly md5 checks
            var pathComparer = new FilePathComparer(prefixPathA: dirA.FullName, prefixPathB: dirB.FullName);
            bool areIdentical = listA.SequenceEqual(listB, pathComparer);
            if (!areIdentical)
                return false;

            // no path (or file length) difference, now go for file content
            return listA.SequenceEqual(listB, new FileMd5Comparer());
        }

        /// <summary>
        /// Check if mod is an update of the target mod.
        /// Must have same ModID.
        /// Go by modinfo.json version first and fallback to checksum if versions are equal.
        /// Consider null targets as updatable.
        /// </summary>
        public bool IsUpdateOf(Mod? target)
        {
            if (target is null || target.IsRemoved)
                return true;
            if (target.Modinfo.ModID != Modinfo.ModID)
                return false;

            // compare content when unversioned
            if (target.Version is null && Version is null)
                return !HasSameContentAs(target); // consider same as outdated

            // prefer versioned mods
            if (target.Version is null)
                return true;
            if (Version is null)
                return false;

            if (Version == target.Version)
                return !HasSameContentAs(target); // consider same as outdated

            return Version > target.Version;
        }
        #endregion

        public ModStatus GetStatus()
        {
            return (Attributes.GetByType(AttributeType.ModStatus) as ModStatusAttribute)?.Status ?? ModStatus.Default;
        }
    }
}
