using Imya.Models;
using Imya.Models.NotifyPropertyChanged;
using System.Diagnostics;

using System.Timers;


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

        public static GameSetupManager Instance { get; } = new GameSetupManager();

        //public ModloaderInstallation ModLoader { get; private set; }

        //event for Game Root Path
        public delegate void GameRootPathChangedEventHandler(String newPath);
        public event GameRootPathChangedEventHandler GameRootPathChanged = delegate { };

        public delegate void ModDirectoryNameChangedEventHandler(String newName);
        public event ModDirectoryNameChangedEventHandler ModDirectoryNameChanged = delegate { };

        public delegate void GameStartEventHandler();
        public event GameStartEventHandler GameStarted = delegate { };

        public delegate void GameCloseEventHandler(int GameExitCode, bool IsRegularExit = true);
        public event GameCloseEventHandler GameClosed = delegate { };

        public bool IsGameRunning { get; private set; }

        // File System Watchers
#pragma warning disable IDE0052 // Never used, but we want to keep them until GameSetupManager dies
        private FileSystemWatcher? ModDirectoryWatcher;
        // private FileSystemWatcher ModLoaderWatcher; // This is TODO
#pragma warning restore IDE0052

        public GameSetupManager()
        {
            GameStarted += () => IsGameRunning = true;
            GameClosed += (x,y) => IsGameRunning = false;
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

        public String ProfilesDirectoryName = "profiles";

        public bool IsValidSetup
        {
           get { return _isValid; }
           private set { _isValid = value; OnPropertyChanged(nameof(IsValidSetup)); }
        }
        private bool _isValid;

        public bool IsModloaderInstalled
        {
            get => _isModloaderInstalled;
            private set {
                _isModloaderInstalled = value;
                OnPropertyChanged(nameof(IsModloaderInstalled));
            }
        }
        private bool _isModloaderInstalled = false;

        public String DownloadDirectory
        {
            get => _downloadDirectory;
            private set {
                _downloadDirectory = value;
                OnPropertyChanged(nameof(DownloadDirectory));
            }
        }
        private String _downloadDirectory = String.Empty;

        #region DIRECTORY_PATH_REQUESTS

        public String GetModDirectory()
        {
            return Path.Combine(GameRootPath, ModDirectoryName);
        }

        public String GetProfilesDirectory()
        {
            return Path.Combine(GameRootPath, ProfilesDirectoryName);
        }

        #endregion

        #region REGISTERING_API

        /// <summary>
        /// Set download directory. Relative to executable.
        /// </summary>
        public void SetDownloadDirectory(string downloadDirectory)
        {
            DownloadDirectory = downloadDirectory;
        }

        public void SetGamePath(String gamePath, bool autoSearchIfInvalid = false)
        {
            String executablePath = Path.Combine(gamePath, "Bin\\Win64\\Anno1800.exe");
            if (!File.Exists(executablePath) && autoSearchIfInvalid)
            {
                var foundGamePath = GameScanner.GetInstallDirFromRegistry() ?? "";
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

        public void SetModDirectoryName(String ModDirectoryName)
        { 
            this.ModDirectoryName = ModDirectoryName;
            ModDirectoryNameChanged(ModDirectoryName);

            Console.WriteLine($"Changed Mod Directory Name to {ModDirectoryName}");
        }

        #endregion

        public void LockGame()
        {
            IsGameRunning = true;
        }

        public void UnlockGame() 
        {
            IsGameRunning = false;
        }

        public bool IsUnlocked()
        {
            return !IsGameRunning;
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

        public void UpdateModloaderInstallStatus()
        {
            IsModloaderInstalled = CheckInstallation();
        }

        private bool CheckInstallation()
        {
            if (ExecutableDir == null) return false;

            var ubiPython = Path.Combine(ExecutableDir, "python35_ubi.dll");
            var python = Path.Combine(ExecutableDir, "python35.dll");
            var executable = ExecutablePath;
            if (File.Exists(ubiPython) && File.Exists(python) && File.Exists(executable))
            {
                // when the executable is a lot newer than ubiPython, then it either got repaired or updated
                // chances are you need an update, but there's no concrete action that you could do here
                // IsPotentiallyOutdated = File.GetLastWriteTimeUtc(executable) > File.GetLastWriteTimeUtc(ubiPython).AddHours(1);

                // mod loader has python and ubiPython at roughly the same time
                // in case python is newer chances are it got repaired.
                // consider it to be not installed in that case.
                // note: will give false results if you repair right before you download a super fresh mod loader release
                // TODO add hash-based check. remove the false result, but is only applicable when downloaded via imya
                return File.GetLastWriteTimeUtc(python) <= File.GetLastWriteTimeUtc(ubiPython).AddMinutes(20);
            }

            return false;
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

        public void StartGame()
        {
            if (ExecutablePath == null)
                return;

            _ = Task.Run(async () =>
            {
                using Process process = new Process();
                process.StartInfo.FileName = ExecutablePath;
                process.EnableRaisingEvents = true;
                process.Exited += OnGameLaunchComplete;
                process.Start();

                Console.WriteLine("Anno 1800 started.");

                await process.WaitForExitAsync();
            }
            );
            //This is worthless. 1800.exe starts uplay.exe (which starts 1800.exe again)
            //and commits suicide before the game window opening.
            //we can just as well do nothing, same result.

        }

        private void OnGameLaunchComplete(object sender, EventArgs e)
        {
            Console.WriteLine($"Start Process exited! Starting Game Scan");

            _ = Task.Run (
                async () =>
                {
                    Process? RunningGame = await ScanForRunningGameAsync(30);

                    if (RunningGame is null) return;

                    RunningGame.EnableRaisingEvents = true;
                    RunningGame.Exited += OnGameExit;

                    GameStarted.Invoke();

                    await RunningGame.WaitForExitAsync();
                } 
            );
        }

        private void OnGameExit(object sender, EventArgs e)
        {
            var process = sender as Process;
            Console.WriteLine($"Anno 1800 exited with Code {process?.ExitCode}");

            int exitCode = process?.ExitCode ?? -1;
            GameClosed.Invoke(exitCode);
        }

        /// <summary>
        /// Asynchronously searches for a running Anno 1800 process 
        /// </summary>
        /// <param name="TimeoutInSeconds"></param>
        /// <returns></returns>
        public async Task<Process?> ScanForRunningGameAsync(int TimeoutInSeconds)
        {
            Process? process = null;
            
            using var periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(1));
            for (int i = TimeoutInSeconds; 
                    process is null && 
                    await periodicTimer.WaitForNextTickAsync() 
                    && i > 0; i--  )
            {
                GameScanner.TryGetRunningGame(out process);
            }

            Console.WriteLine(process is not null ? "running process found!" : "failed to retrieve any running process");

            if (process is null)
            {
                GameClosed.Invoke(-1, false);
            }

            return process;
        }
    }
}
