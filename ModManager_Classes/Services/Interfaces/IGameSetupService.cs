using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Services.Interfaces
{
    /// <summary>
    /// Validates and setups the basics of a modded game installation.
    /// 
    /// - Mods folder
    /// - Modloader
    /// - 
    /// </summary>

    public enum GamePlatform { Steam, Agnostic }

    public enum ModloaderInstallationState { Installed, Uninstalled, Deactivated }

    public interface IGameSetupService
    {
        public delegate void GameRootPathChangedEventHandler(string newPath);
        public event GameRootPathChangedEventHandler GameRootPathChanged;

        public delegate void ModDirectoryNameChangedEventHandler(string newName);
        public event ModDirectoryNameChangedEventHandler ModDirectoryNameChanged;

        bool IsGameRunning { get; set; }
        bool IsModloaderInstalled { get; }
        bool IsValidSetup { get; }
        string GameRootPath { get; }
        string? ExecutablePath { get; }
        string ModDirectoryName { get; }

        GamePlatform GamePlatform { get; }
        ModloaderInstallationState ModloaderState { get; }

        string GetModDirectory();
        void SetGamePath(string gamePath, bool autoSearchifInvalid = false);
        void SetModDirectoryName(string modDirectoryName);
        void DeleteCache();

        bool NeedsModloaderRemoval();
        void RemoveModloader();
    }
}
