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

        public String GAME_ROOT_PATH { get => _game_root_path;
            private set
            {
                _game_root_path = value;
                OnPropertyChanged(nameof(GAME_ROOT_PATH));
            }
        }
        private String _game_root_path;

        public String MOD_DIRECTORY_NAME {
            get => _mod_directory_name;
            private set 
            { 
                _mod_directory_name = value;
                OnPropertyChanged(nameof(MOD_DIRECTORY_NAME));
            }
        }
        private String _mod_directory_name;

        #region DIRECTORY_PATH_REQUESTS

        public String GetGameWin64Dir()
        {
            return Path.Combine(GAME_ROOT_PATH, "Bin", "Win64");
        }

        public String GetModDirectory()
        {
            return Path.Combine(GAME_ROOT_PATH, MOD_DIRECTORY_NAME);
        }

        #endregion

        #region REGISTERING_API

        public void RegisterGameRootPath(String game_root_path)
        {
            GAME_ROOT_PATH = game_root_path;
            GameRootPathChanged(game_root_path);
            CreateWatchers();
        }

        public void RegisterModDirectoryName(String ModDirectoryName)
        { 
            MOD_DIRECTORY_NAME = ModDirectoryName;
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
            ModDirectoryWatcher = CreateWatcher(Path.Combine(GAME_ROOT_PATH));
        }

        public String GetAnno1800ExePath()
        {
            return Path.Combine(GAME_ROOT_PATH, "Bin", "Win64", "Anno1800.exe");
        }


        public bool RootPathExists()
        {
            return Directory.Exists(GAME_ROOT_PATH);
        }

        public bool Anno1800ExeExists()
        {
            return File.Exists(GetAnno1800ExePath());
        }

        public bool MaindataIsValid()
        {
            String MaindataPath = Path.Combine(GAME_ROOT_PATH, "maindata");
            return 
                Directory.Exists(MaindataPath) &&
                CheckMaindata(MaindataPath);
        }

        public bool ModLoaderIsInstalled()
        {
            return 
                File.Exists(Path.Combine(GAME_ROOT_PATH, "Bin", "Win64", "python35.dll")) &&
                File.Exists(Path.Combine(GAME_ROOT_PATH, "Bin", "Win64", "python35_ubi.dll"));
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
                if (!Directory.Exists(Path.Combine(GAME_ROOT_PATH, MaindataPath, s)))
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
            var Path = GetAnno1800ExePath();
            if (File.Exists(Path))
            {
                using (Process process = new Process())
                { 
                    process.StartInfo.FileName = Path;
                    process.EnableRaisingEvents = true;
                    process.Exited += OnGameExit;
                    process.Start();

                    Console.WriteLine("Anno 1800 started.");

                    //This is worthless. 1800.exe starts uplay.exe (which starts 1800.exe again)
                    //and commits suicide before the game window opening.
                    //we can just as well do nothing, same result.
                    process.WaitForExit();
                }
            }
        }

        private void OnGameExit(object sender, EventArgs e)
        {
            Console.WriteLine($"Anno 1800 exited!");
        }

    }
}
