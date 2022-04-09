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

namespace Imya.UI.Views
{
    /// <summary>
    /// Main view to install mods.
    /// </summary>
    /// 
    public partial class InstallationView : UserControl, INotifyPropertyChanged
    {
        public ObservableCollection<Installation> RunningInstallations { get; } = new();

        public TextManager TextManager { get; } = TextManager.Instance;
        public GameSetupManager GameSetup { get; } = GameSetupManager.Instance;

        public ZipInstallationOptions Options { get; } = new();

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

            if (GameSetup.ModLoader.IsInstalled)
            {
                InstallStatus = ModLoaderStatus.Installed;
                // TODO async update check
                // InstallStatusText = "checking...";
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

        private List<Task<ZipInstallation>> CreateInstallationTasks(IEnumerable<String> Filenames)
        {
            List<Task<ZipInstallation>> InstallationTasks = new();

            foreach (var Filename in Filenames)
            {
                if (!IsRunningInstallation(Filename))
                {
                    var InstallationTask = new ZipInstallation(Filename, Properties.Settings.Default.DownloadDir, Options);
                    //add to displayed list
                    RunningInstallations.Add(InstallationTask);
                    //add to list of tasks for parallel async
                    InstallationTasks.Add(InstallationTask.RunUnpack());
                }
                else
                {
                    Console.WriteLine($"Installation Already Running: {Filename}");
                }
            }

            return InstallationTasks;
        }

        private async void OnInstallFromZipAsync(object sender, RoutedEventArgs e)
        {
            if (ModCollection.Global is null) return;

            var dialog = CreateOpenFileDialog();
            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            //IsInstalling = true;

            var InstallationTasks = CreateInstallationTasks(dialog.FileNames);

            IEnumerable<ZipInstallation>? TaskResults = await Task.WhenAll(InstallationTasks);

            foreach (var _task in TaskResults)
            {
                await _task.RunMove();
                RemoveZipInstallation(_task);
            }

            //IsInstalling = false;
            
            //disable this for now :) 
            //MainViewController.Instance.SetView(View.MOD_ACTIVATION);

            //nuked comments (I am sorry jakob)

            // TODO current progress assumes all zip files take similarily long
            //      this can be improved by giving absolute progress vs MB size for example
            //      but that's an update to be done when zip actually supports progress

        }

        private void RemoveZipInstallation(ZipInstallation x)
        {
            bool success = App.Current.Dispatcher.Invoke(() => RunningInstallations.Remove(x));
            Console.WriteLine(success ? $"Successfully removed {x}" : $"Not able to remove {x}");
        }

        private bool IsRunningInstallation(String SourceFilepath) => RunningInstallations.Any(x => x is ZipInstallation && ((ZipInstallation)x).SourceFilepath.Equals(SourceFilepath));

        //private bool IsRunningModloaderInstallation(String SourceFilepath) => RunningInstallations.Any(x => x is ModLoaderInstallation);

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

        public async void OnInstallModLoader(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Installing Modloader");
            ModloaderDownloadButton.IsEnabled = false;
            InstallStatus = ModLoaderStatus.Installing;

            await GameSetup.ModLoader.InstallAsync();

            ModloaderDownloadButton.IsEnabled = true;
            if (GameSetup.ModLoader.IsInstalled)
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
