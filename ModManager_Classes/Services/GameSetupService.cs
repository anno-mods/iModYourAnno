using Imya.Models;
using Imya.Models.GameLauncher;
using Imya.Models.NotifyPropertyChanged;
using Imya.Services.Interfaces;
using Imya.Utils;
using System;
using System.Diagnostics;
using System.Timers;

namespace Imya.Services
{
    /// <summary>
    /// Validates and setups the basics of a modded game installation.
    /// 
    /// - Mods folder
    /// - Modloader
    /// - 
    /// 
    /// </summary>

    public class GameSetupService : PropertyChangedNotifier, IGameSetupService
    {
        private InstallationValidator _validator;
        private GameScanner _scanner;

        #region EVENTS
        public event IGameSetupService.GameRootPathChangedEventHandler GameRootPathChanged = delegate { };
        public event IGameSetupService.ModDirectoryNameChangedEventHandler ModDirectoryNameChanged = delegate { };

        #endregion

        // File System Watchers
#pragma warning disable IDE0052 // Never used, but we want to keep them until GameSetupManager dies
        private FileSystemWatcher? ModDirectoryWatcher;
        // private FileSystemWatcher ModLoaderWatcher; // This is TODO
#pragma warning restore IDE0052

        public bool IsGameRunning
        {
            get => _isGameRunning;
            set
            {
                _isGameRunning = value;
                OnPropertyChanged(nameof(IsGameRunning));
            }
        }
        private bool _isGameRunning;

        public string GameRootPath
        {
            get => _gameRootPath;
            private set
            {
                _gameRootPath = value;
                OnPropertyChanged(nameof(GameRootPath));
            }
        }
        private string _gameRootPath;

        public string? ExecutablePath { get; private protected set; }
        public string? ExecutableDir { get; private protected set; }

        public string ModDirectoryName
        {
            get => _modDirectoryName;
            private set
            {
                _modDirectoryName = value;
                OnPropertyChanged(nameof(ModDirectoryName));
            }
        }
        private string _modDirectoryName;

        public bool IsValidSetup
        {
            get { return _isValid; }
            private set { _isValid = value; OnPropertyChanged(nameof(IsValidSetup)); }
        }
        private bool _isValid;

        public GamePlatform GamePlatform
        {
            get
            {
                if (ExecutableDir is null)
                    return GamePlatform.Agnostic;

                var path = Path.Combine(ExecutableDir, "steam_api64.dll");
                return File.Exists(path) ? GamePlatform.Steam : GamePlatform.Agnostic;
            }
        }

        public bool IsModloaderInstalled => ModloaderState == ModloaderInstallationState.Installed;

        public ModloaderInstallationState ModloaderState
        {
            get => _modloaderState;
            set
            {
                _modloaderState = value;
                OnPropertyChanged(nameof(ModloaderState));
            }
        }
        private ModloaderInstallationState _modloaderState = ModloaderInstallationState.Uninstalled;

        public GameSetupService()
        {
            _scanner = new GameScanner();
        }

        #region DIRECTORY_RELATED

        public string GetModDirectory() => Path.Combine(GameRootPath, ModDirectoryName);

        /// <summary>
        /// Set download directory. Relative to executable.
        /// </summary>
        public void SetGamePath(string gamePath, bool autoSearchIfInvalid = false)
        {
            string executablePath = Path.Combine(gamePath, "Bin", "Win64", "Anno1800.exe");
            if (!File.Exists(executablePath) && autoSearchIfInvalid)
            {
                var foundGamePath = _scanner.GetInstallDirFromRegistry() ?? "";
                executablePath = Path.Combine(foundGamePath, "Bin", "Win64", "Anno1800.exe");
                if (File.Exists(executablePath))
                    gamePath = foundGamePath; // only replace if found, otherwise keep "wrong" path in the settings.
            }

            GameRootPath = gamePath;
            _validator = new InstallationValidator(GameRootPath);

            if (File.Exists(executablePath))
            {
                ExecutablePath = executablePath;
                ExecutableDir = Path.Combine(gamePath, "Bin", "Win64");
                IsValidSetup = true;
            }
            else
            {
                ExecutablePath = null;
                ExecutableDir = null;
                IsValidSetup = false;
            }

            GameRootPathChanged(gamePath);
            CreateWatchers();
        }

        public void SetModDirectoryName(string ModDirectoryName)
        {
            this.ModDirectoryName = ModDirectoryName;
            ModDirectoryNameChanged(ModDirectoryName);

            Console.WriteLine($"Changed Mod Directory Name to {ModDirectoryName}");
        }

        #endregion

        private void CreateWatchers() => ModDirectoryWatcher = CreateWatcher(Path.Combine(GameRootPath));

        [Obsolete]
        public void UpdateModloaderInstallStatus() => ModloaderState = _validator.CheckModloaderInstallState();
        [Obsolete]
        public void EnsureModloaderActivation(bool desired)
        {
            UpdateModloaderInstallStatus();

            if (desired && ModloaderState == ModloaderInstallationState.Deactivated)
            {
                ActivateModloader();
                return;
            }
            if (!desired && ModloaderState == ModloaderInstallationState.Installed)
                DeactivateModloader();
        }

        public void DeleteCache()
        {
            var cache = Path.Combine(GetModDirectory(), ".cache");
            _ = DirectoryEx.TryDelete(cache);
        }

        [Obsolete]
        private void ActivateModloader()
        {
            if (ModloaderState != ModloaderInstallationState.Deactivated) return;
            try
            {
                File.Move(Path.Combine(ExecutableDir, "python35.dll"), Path.Combine(ExecutableDir, "python35_ubi.dll"));
                File.Move(Path.Combine(ExecutableDir, "modloader.dll"), Path.Combine(ExecutableDir, "python35.dll"));

                ModloaderState = ModloaderInstallationState.Installed;
            }
            catch (Exception e)
            {
                Console.WriteLine("Modloader Activation unsuccessful");
                return;
            }
        }
        [Obsolete]
        private void DeactivateModloader()
        {
            if (!IsModloaderInstalled) return;

            try
            {
                //move python35 to modloader and python35_ubi to python35. modloader.dll is junk anyway, just override it.
                File.Move(Path.Combine(ExecutableDir, "python35.dll"), Path.Combine(ExecutableDir, "modloader.dll"), true);
                File.Move(Path.Combine(ExecutableDir, "python35_ubi.dll"), Path.Combine(ExecutableDir, "python35.dll"));

                ModloaderState = ModloaderInstallationState.Deactivated;
            }
            catch (Exception e)
            {
                Console.WriteLine("Modloader Deactivation unsuccessful");
                return;
            }
        }

        /// <summary>
        /// Check if python35_ubi.dll is there.
        /// </summary>
        public bool NeedsModloaderRemoval()
        {
            if (ExecutableDir is null || !Directory.Exists(ExecutableDir)) return false;
            return File.Exists(Path.Combine(ExecutableDir, "python35_ubi.dll"));
        }

        /// <summary>
        /// Rename python35_ubi.dll to python35.dll
        /// </summary>
        public void RemoveModloader()
        {
            if (!NeedsModloaderRemoval()) return;

            try
            {
                File.Delete(Path.Combine(ExecutableDir!, "python35.dll"));
                File.Move(Path.Combine(ExecutableDir!, "python35_ubi.dll"), Path.Combine(ExecutableDir!, "python35.dll"));
                ModloaderState = ModloaderInstallationState.Uninstalled;
            }
            catch (Exception)
            {
                Console.WriteLine("Modloader deactivation unsuccessful");
            }
        }

        private FileSystemWatcher? CreateWatcher(string pathToWatch)
        {
            if (!Directory.Exists(pathToWatch))
            {
                // There are many other things that can go wrong besides a non-existant path, but let's take this for now
                return null;
            }

            return new FileSystemWatcher
            {
                Path = pathToWatch,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.CreationTime,
                IncludeSubdirectories = true,
                Filter = "*"
            };
        }
    }
}
