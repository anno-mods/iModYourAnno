using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Imya.Models;
using Imya.Utils;

namespace Imya.UI.Views
{
    /// <summary>
    /// Main view to install mods.
    /// </summary>
    public partial class InstallationView : UserControl, INotifyPropertyChanged, IProgress<float>
    {
        public TextManager TextManager { get; } = TextManager.Instance;
        public GameSetupManager GameSetup { get; } = GameSetupManager.Instance;

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

        public float Progress
        {
            get => _progress;
            set => SetProperty(ref _progress, value);
        }
        private float _progress = 0.1f;

        public bool IsInstalling
        {
            get => _isInstalling;
            set => SetProperty(ref _isInstalling, value);
        }
        private bool _isInstalling = false;

        public bool AllowOldToOverwrite
        {
            get => _allowOldToOverwrite;
            set => SetProperty(ref _allowOldToOverwrite, value);
        }
        private bool _allowOldToOverwrite = false;
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

        public void Report(float value)
        {
            Progress = _progressRange.Item1 + value * (_progressRange.Item2 - _progressRange.Item1);
        }
        private (float, float) _progressRange = (0, 1);

        private void OnInstallFromZip(object sender, RoutedEventArgs e)
        {
            if (ModCollection.Global is null) return;

            var dialog = new System.Windows.Forms.OpenFileDialog
            {
                Filter = "Zip Archives (*.zip)|*.zip",
                RestoreDirectory = true, // TODO keep location separate from game path dialog, it's annoying!
                Multiselect = true
            };
            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            Progress = 0;
            IsInstalling = true;
            var allowOldToOverwrite = AllowOldToOverwrite;

            _ = Task.Run(async () =>
            {
                // TODO current progress assumes all zip files take similarily long
                //      this can be improved by giving absolute progress vs MB size for example
                //      but that's an update to be done when zip actually supports progress
                var progressRangeOneItem = 0.9f / dialog.FileNames.Length;

                var modCollections = new List<ModCollection>();
                for (var (idx, iter) = (0, dialog.FileNames.GetEnumerator()); iter.MoveNext(); idx++)
                {
                    var filePath = (string)iter.Current;
                    _progressRange = (idx * progressRangeOneItem, (idx+1) * progressRangeOneItem);

                    Console.WriteLine($"Extract zip: {filePath}");
                    var result = await ModInstaller.ExtractZipAsync(filePath, 
                        Path.Combine(Directory.GetCurrentDirectory(), Properties.Settings.Default.DownloadDir),
                        this);
                    if (result != null)
                        modCollections.Add(result);
                }

                Progress = 0.9f;

                // TODO progress for MoveIntoAsync should be done per mod

                foreach (var collection in modCollections)
                {
                    Console.WriteLine($"Install zip: {collection.ModsPath}");
                    await ModCollection.Global.MoveIntoAsync(collection, 
                        AllowOldToOverwrite: allowOldToOverwrite);
                }

                // TODO switching to activation view is not fun when the user already switched himself in the meantime
                MainViewController.Instance.SetView(View.MOD_ACTIVATION);
                Progress = 0;
                IsInstalling = false;
            });
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
