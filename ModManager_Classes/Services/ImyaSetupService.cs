using Imya.Models.Mods;
using Imya.Models.NotifyPropertyChanged;
using Imya.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Services
{
    public class ImyaSetupService : PropertyChangedNotifier, IImyaSetupService
    {
        private static string WorkingDirectory = ".imya";
        private static string ProfilesDirectory = "profiles";
        private static string DownloadDirectory = "download";
        private static string TweakDirectory = "tweaks";
        private static string UnpackDirectory = ".unpack";

        public IGameSetupService _gameSetup;

        public string WorkingDirectoryPath { get => Path.Combine(_gameSetup.GameRootPath, WorkingDirectory); }
        public string ProfilesDirectoryPath { get => Path.Combine(_gameSetup.GameRootPath, WorkingDirectory, ProfilesDirectory); }
        public string DownloadDirectoryPath { get => Path.Combine(_gameSetup.GameRootPath, WorkingDirectory, DownloadDirectory); }
        //the fallback currently stores the tweaks elsewhere. I have no idea why the gamesetup isn't injected properly here.
        public string TweakDirectoryPath { get => Path.Combine(_gameSetup.GameRootPath, WorkingDirectory, TweakDirectory); }
        public string UnpackDirectoryPath { get => Path.Combine(_gameSetup.GameRootPath, WorkingDirectory, UnpackDirectory); }

        public ModCollection GlobalModCollection { get; set; }

        public ImyaSetupService(IGameSetupService gameSetup)
        {
            _gameSetup = gameSetup;
            if (Directory.Exists(_gameSetup.GameRootPath))
            {
                Init();
            }
            _gameSetup.GameRootPathChanged += OnGameRootPathChanged;
        }

        private void OnGameRootPathChanged(string GameRootPath)
        {
            if (Directory.Exists(GameRootPath))
            {
                Init();
            }
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
