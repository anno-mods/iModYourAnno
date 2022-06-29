using Imya.Models.NotifyPropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Utils
{
    public class ImyaSetupManager : PropertyChangedNotifier
    {
        public static String WorkingDirectory = ".imya";
        public static String ProfilesDirectory = "profiles";
        public static String DownloadDirectory = "download";
        public static String TweakDirectory = "tweaks";
        public static String UnpackDirectory = ".unpack";

        public static ImyaSetupManager Instance = new ImyaSetupManager();

        public GameSetupManager GameSetup = GameSetupManager.Instance;

        public String WorkingDirectoryPath { get => Path.Combine(GameSetup.GameRootPath, WorkingDirectory); }
        public String ProfilesDirectoryPath { get => Path.Combine(GameSetup.GameRootPath, WorkingDirectory, ProfilesDirectory); }
        public String DownloadDirectoryPath { get => Path.Combine(GameSetup.GameRootPath, WorkingDirectory, DownloadDirectory); }
        public String TweakDirectoryPath { get => Path.Combine(GameSetup.GameRootPath, WorkingDirectory, TweakDirectory); }
        public String UnpackDirectoryPath { get => Path.Combine(GameSetup.GameRootPath, WorkingDirectory, UnpackDirectory); }

        public ImyaSetupManager() 
        {
            Init();
            GameSetup.GameRootPathChanged += OnGameRootPathChanged;
        }

        private void OnGameRootPathChanged(String GameRootPath)
        {
            Init();
        }

        public void Init()
        {
            Directory.CreateDirectory(WorkingDirectoryPath);
            Directory.CreateDirectory(ProfilesDirectoryPath);
            Directory.CreateDirectory(DownloadDirectoryPath);
            Directory.CreateDirectory(TweakDirectoryPath);
            Directory.CreateDirectory(UnpackDirectoryPath);
        }
    }
}
