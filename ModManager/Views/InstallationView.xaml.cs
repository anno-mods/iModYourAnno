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
using Imya.UI.Utils;
using Imya.Models.Options;
using Imya.GithubIntegration.Download;

namespace Imya.UI.Views
{
    /// <summary>
    /// Main view to install mods.
    /// </summary>
    /// 
    public partial class InstallationView : UserControl, INotifyPropertyChanged
    {
        public static InstallationView? Instance { get; private set; }

        public TextManager TextManager { get; } = TextManager.Instance;
        public GameSetupManager GameSetup { get; } = GameSetupManager.Instance;
        public InstallationSetup Installer { get; } = new InstallationSetup();
        public InstallationManager InstallationManager { get; private set; }

        public Properties.Settings Settings { get; } = Properties.Settings.Default;

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

        public bool IsInstalling
        {
            get => _isInstalling;
            set => SetProperty(ref _isInstalling, value);
        }
        private bool _isInstalling = false;

        #endregion


        public InstallationView()
        {
            Instance = this;

            InitializeComponent();
            DataContext = this;

            InstallationManager = new InstallationManager(Installer);
            TextManager.LanguageChanged += OnLanguageChanged;

            if (GameSetup.IsModloaderInstalled)
            {
                InstallStatus = ModLoaderStatus.Installed;
            }

        }


        

        private void OnInstallFromGithub(object sender, RoutedEventArgs e)
        {
            MainViewController.Instance.SetView(View.GITHUB_BROWSER);
        }

        public async void OnInstallModLoader(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Installing Modloader");
            ModloaderDownloadButton.IsEnabled = false;
            InstallStatus = ModLoaderStatus.Installing;

            var Result = await InstallationManager.RunModloaderInstallAsync();

            switch (Result.ResultType)
            {
                case InstallationResultType.InstallationAlreadyRunning:
                    PopupCreator.CreateInstallationAlreadyRunningPopup().ShowDialog();
                    break;
                case InstallationResultType.Exception:
                    PopupCreator.CreateGithubExceptionPopup(Result.Exception!).ShowDialog();
                    break;
                default:
                    Console.WriteLine("Installation successful");
                    break;
            }

            ModloaderDownloadButton.IsEnabled = true;
            GameSetup.UpdateModloaderInstallStatus();
            InstallStatus = GameSetup.IsModloaderInstalled ? ModLoaderStatus.Installed : ModLoaderStatus.NotInstalled;
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

        public IText Localized => TextManager.Instance[_value];
    }
}
