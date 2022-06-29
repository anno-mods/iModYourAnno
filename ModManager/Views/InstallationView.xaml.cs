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

namespace Imya.UI.Views
{
    /// <summary>
    /// Main view to install mods.
    /// </summary>
    /// 
    public partial class InstallationView : UserControl, INotifyPropertyChanged
    {
        public TextManager TextManager { get; } = TextManager.Instance;
        public GameSetupManager GameSetup { get; } = GameSetupManager.Instance;

        public ModInstallationOptions Options { get; } = new();
        public Installer Installer { get; } = new Installer();

        IRepositoryInfoProvider RepoInfoProvider = new StaticRepositoryInfoProvider();

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
            InitializeComponent();
            DataContext = this;

            TextManager.LanguageChanged += OnLanguageChanged;

            if (GameSetup.IsModloaderInstalled)
            {
                InstallStatus = ModLoaderStatus.Installed;
            }
        }

        private System.Windows.Forms.OpenFileDialog CreateOpenFileDialog()
        {
            return new System.Windows.Forms.OpenFileDialog
            {
                Filter = "Zip Archives (*.zip)|*.zip",
                RestoreDirectory = true, // TODO keep location separate from game path dialog, it's annoying!
                Multiselect = true
            };
        }

        public void OnOpenGamePath(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                GameSetup.SetGamePath(dialog.SelectedPath);
                // TODO validity feedback?
                Properties.Settings.Default.GameRootPath = dialog.SelectedPath;
                Properties.Settings.Default.Save();
            }
        }

        private async void OnInstallFromGithub(object sender, RoutedEventArgs e)
        {
            GenericOkayPopup popup = new GenericOkayPopup();
            popup.MESSAGE = new SimpleText("Installing Taludas/WholesomeHaciendaOverhaul from Github now!");
            popup.ShowDialog();

            var InstallationTask = Installer.CreateModInstallationTask(RepoInfoProvider.GetSingle(), Options);
            await Installer.ProcessAsync(InstallationTask);
        }

        private async void OnInstallFromZipAsync(object sender, RoutedEventArgs e)
        {
            if (ModCollection.Global is null) return;

            var dialog = CreateOpenFileDialog();
            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            var InstallationTasks = Installer.CreateInstallationTasks(dialog.FileNames, Options);
            await Installer.ProcessParallelAsync(InstallationTasks);
        }

        public async void OnInstallModLoader(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Installing Modloader");
            ModloaderDownloadButton.IsEnabled = false;
            InstallStatus = ModLoaderStatus.Installing;

            var installation = Installer.CreateModloaderInstallationTask();
            await Installer.ProcessAsync(installation);

            ModloaderDownloadButton.IsEnabled = true;
            GameSetup.UpdateModloaderInstallStatus();
            InstallStatus = ModLoaderStatus.Installed;

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {

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

        public IText Localized => TextManager.Instance[_value];
    }
}
