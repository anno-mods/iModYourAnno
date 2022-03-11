using Imya.Models.NotifyPropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imya.GithubIntegration;
using Imya.Models;
using System.Diagnostics;

namespace Imya.Utils
{
    /// <summary>
    /// Validates and setups the basics of a modded game installation.
    /// 
    /// - Mods folder
    /// - Modloader
    /// - 
    /// 
    /// </summary>
    public class GameSetupManager : PropertyChangedNotifier
    {
        private static int MAX_RDA_INDEX = 21;

        public static GameSetupManager Instance { get; private set; }

        //event for Game Root Path
        public delegate void GameRootPathChangedEventHandler(String newPath);
        public event GameRootPathChangedEventHandler GameRootPathChanged = delegate { };

        public delegate void ModDirectoryNameChangedEventHandler(String newName);
        public event ModDirectoryNameChangedEventHandler ModDirectoryNameChanged = delegate { };

        private bool isLocked = false;

        // File System Watchers
#pragma warning disable IDE0052 // Never used, but we want to keep them until GameSetupManager dies
        private FileSystemWatcher? ModDirectoryWatcher;
        // private FileSystemWatcher ModLoaderWatcher; // This is TODO
#pragma warning restore IDE0052

        public GameSetupManager()
        {
            Instance ??= this;
        }

        public String GameRootPath { get => _gameRootPath;
            private set
            {
                _gameRootPath = value;
                OnPropertyChanged(nameof(GameRootPath));
            }
        }
        private String _gameRootPath;

        public String? ExecutablePath { get; private protected set; }
        public String? ExecutableDir { get; private protected set; }

        public String ModDirectoryName {
            get => _modDirectoryName;
            private set 
            { 
                _modDirectoryName = value;
                OnPropertyChanged(nameof(ModDirectoryName));
            }
        }
        private String _modDirectoryName;

        public bool IsValidSetup
        {
           get { return _isValid; }
           private set { _isValid = value; OnPropertyChanged(nameof(IsValidSetup)); }
        }
        private bool _isValid;

        #region DIRECTORY_PATH_REQUESTS

        public String GetModDirectory()
        {
            return Path.Combine(GameRootPath, ModDirectoryName);
        }

        #endregion

        #region REGISTERING_API

        public void SetGamePath(String gamePath, bool autoSearchIfInvalid = false)
        {
            String executablePath = Path.Combine(gamePath, "Bin\\Win64\\Anno1800.exe");
            if (!File.Exists(executablePath) && autoSearchIfInvalid)
            {
                var foundGamePath = GamePathFinder.GetInstallDirFromRegistry() ?? "";
                executablePath = Path.Combine(foundGamePath, "Bin\\Win64\\Anno1800.exe");
                if (File.Exists(executablePath))
                    gamePath = foundGamePath; // only replace if found, otherwise keep "wrong" path in the settings.
            }

            GameRootPath = gamePath;

            if (File.Exists(executablePath))
            {
                ExecutablePath = executablePath;
                ExecutableDir = Path.Combine(gamePath, "Bin\\Win64");
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

        public void RegisterModDirectoryName(String ModDirectoryName)
        { 
            this.ModDirectoryName = ModDirectoryName;
            ModDirectoryNameChanged(ModDirectoryName);
        }

        #endregion

        public void LockGame()
        { 
            isLocked = true;
        }

        public void UnlockGame() 
        {
            isLocked = false;
        }

        public bool IsUnlocked()
        {
            return !isLocked;
        }

        private void CreateWatchers()
        {
            ModDirectoryWatcher = CreateWatcher(Path.Combine(GameRootPath));
        }

        public bool MaindataIsValid()
        {
            String MaindataPath = Path.Combine(GameRootPath, "maindata");
            return 
                Directory.Exists(MaindataPath) &&
                CheckMaindata(MaindataPath);
        }

        public bool ModLoaderIsInstalled()
        {
            return 
                File.Exists(Path.Combine(GameRootPath, "Bin", "Win64", "python35.dll")) &&
                File.Exists(Path.Combine(GameRootPath, "Bin", "Win64", "python35_ubi.dll"));
        }

        private bool CheckMaindata(String MaindataPath)
        {
            List<String> BuildPaths = new List<String>();

            for (int i = 0; i <= MAX_RDA_INDEX; i++)
            {
                BuildPaths.Add($"data{i}.rda");
            }

            bool allExist = true;
            foreach (String s in BuildPaths)
            {
                if (!Directory.Exists(Path.Combine(GameRootPath, MaindataPath, s)))
                {
                    allExist = false;
                }
            }

            return allExist; 
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

        public IEnumerable<String> getFilesWithExtension(Mod m, String Extension)
        {
            String ModDir = Path.Combine(GetModDirectory(), (m.Active ? "" : "-") + m.DirectoryName);
            var files = Directory.EnumerateFiles(ModDir, $"*.{Extension}", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                Console.WriteLine(file);
                yield return file;
            }
        }

        public void StartGame()
        {
            if (ExecutablePath == null)
                return;

            using Process process = new Process();
            process.StartInfo.FileName = ExecutablePath;
            process.EnableRaisingEvents = true;
            process.Exited += OnGameExit;
            process.Start();

            Console.WriteLine("Anno 1800 started.");

            //This is worthless. 1800.exe starts uplay.exe (which starts 1800.exe again)
            //and commits suicide before the game window opening.
            //we can just as well do nothing, same result.
            process.WaitForExit();
        }

        private void OnGameExit(object sender, EventArgs e)
        {
            Console.WriteLine($"Anno 1800 exited!");
        }

    }
}
