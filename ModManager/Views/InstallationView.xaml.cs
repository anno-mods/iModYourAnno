using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Imya.Models;
using Imya.Utils;

using System.Linq;
using Imya.Models.Installation;
using Imya.UI.Popup;
using Imya.GithubIntegration;
using Imya.Models.Options;
using Imya.GithubIntegration.Download;
using Imya.GithubIntegration.StaticData;
using Downloader;
using Imya.Services;
using Imya.Services.Interfaces;
using Imya.Texts;
using Imya.UI.Models;
using Imya.Models.Installation.Interfaces;

namespace Imya.UI.Views
{
    /// <summary>
    /// Main view to install mods.
    /// </summary>
    /// 
    public partial class InstallationView : UserControl, INotifyPropertyChanged
    {
        public static InstallationView? Instance { get; private set; }

        public ITextManager TextManager { get; init; }
        public IGameSetupService GameSetup { get; init; }
        public IAppSettings Settings { get; init; }
        public IInstallationService InstallationManager { get; init; }

        public ObservableCollection<IInstallation> PendingDownloads { get; }

        #region notifyable properties

        public ModLoaderStatus InstallStatus
        {
            get => _installStatus;
            private set
            {
                _installStatus = value;
                OnPropertyChanged(nameof(InstallStatus));
            }
        }
        private ModLoaderStatus _installStatus = ModLoaderStatus.NotInstalled;



        #endregion

        public InstallationView(ITextManager textManager, 
            IGameSetupService gameSetupService,
            IAppSettings appSettings,
            IInstallationService installationService)
        {
            TextManager= textManager;
            GameSetup= gameSetupService;
            Settings = appSettings;
            InstallationManager= installationService;

            Instance = this;

            InitializeComponent();
            DataContext = this;

            TextManager.LanguageChanged += OnLanguageChanged;

            if (GameSetup.IsModloaderInstalled)
            {
                InstallStatus = ModLoaderStatus.Installed;
            }            
        }

        public async void OnInstallModLoader(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("This will do fucking nothing forever");
        }

        private void OnLanguageChanged(ApplicationLanguage language)
        {
            // trigger property changes to update text
            OnPropertyChanged(nameof(InstallStatus));
        }

        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler? PropertyChanged = delegate { };
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        private void SetProperty<T>(ref T property, T value, [CallerMemberName] string propertyName = "")
        {
            property = value;
            OnPropertyChanged(propertyName);
        }
        #endregion

        private void PauseButtonClicked(object sender, RoutedEventArgs e)
        {
            var but = sender as Button;
            var pausable = but?.DataContext as IPausable;
            if (pausable is null)
                return;

            if (pausable.IsPaused)
                InstallationManager.Resume();
            else
            {
                InstallationManager.Pause();
            }           
        }

        private async void CancelButtonClicked(object sender, RoutedEventArgs e)
        {
            var but = sender as Button;
            var installation = but?.DataContext as IInstallation;
            if (installation is null)
                return;

            await InstallationManager.CancelAsync(installation);
        }

        private void RemoveButtonClicked(object sender, RoutedEventArgs e)
        {
            var but = sender as Button;
            var installation = but?.DataContext as IDownloadableUnpackableInstallation;
            if (installation is null)
                return;

            InstallationManager.RemovePending(installation);
        }
    }

    /// <summary>
    /// Enum with overwritten ToString to provide localized text.
    /// </summary>
    public class ModLoaderStatus
    {
        public static readonly ModLoaderStatus NotInstalled = new("MODLOADER_NOT_INSTALLED");
        public static readonly ModLoaderStatus Checking = new("MODLOADER_CHECKING");
        public static readonly ModLoaderStatus Installing = new("MODLOADER_INSTALLING");
        public static readonly ModLoaderStatus UpdateAvailable = new("MODLOADER_UPDATE_AVAILABLE");
        public static readonly ModLoaderStatus Installed = new("MODLOADER_INSTALLED");

        private readonly string _value;
        private ModLoaderStatus(string value)
        {
            _value = value;
        }

        public IText Localized => new SimpleText("lolololol");
    }
}
