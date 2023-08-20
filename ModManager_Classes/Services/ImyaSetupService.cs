﻿using Anno.EasyMod.Mods;
using Imya.Models.NotifyPropertyChanged;
using Imya.Services.Interfaces;

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

        public IModCollection GlobalModCollection 
        {
            get => _globalModCollection;
            set
            {
                if (value is null)
                    return; 
                if (_globalModCollection == value)
                    return;

                /*
                if (_globalModCollection is not null)
                {
                    _gameSetup.GameRootPathChanged -= _globalModCollection.OnModPathChanged;
                    _gameSetup.ModDirectoryNameChanged -= _globalModCollection.OnModPathChanged;
                }
                */

                _globalModCollection = value;

                /*
                _gameSetup.GameRootPathChanged += value.OnModPathChanged;
                _gameSetup.ModDirectoryNameChanged += value.OnModPathChanged;
                */

            }
        }
        private IModCollection _globalModCollection; 

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
