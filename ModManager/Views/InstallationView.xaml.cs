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

namespace Imya.UI.Views
{
    /// <summary>
    /// Main view to install mods.
    /// </summary>
    /// 

    public class ModInstallationTask : InstallationTask<ModCollection>
    {
        public ModInstallationTask(String source_file_name) : base(source_file_name) { }

        public override event InstallationTaskCompletedEventHandler InstallationTaskComplete = delegate { };

        public override Task<ModCollection?> RunInstall()
        {
            IsInstalling = true;
            var allowOldToOverwrite = AllowOldToOverwrite;

            return Task.Run(async () =>
            {
                Console.WriteLine($"Extract zip: {SourceFilepath}");
                var result = await ModInstaller.ExtractZipAsync(SourceFilepath,
                    Path.Combine(Directory.GetCurrentDirectory(), Properties.Settings.Default.DownloadDir),
                    this);

                InstallationTaskComplete(this);
                return result;
            }
            );
        }

        public bool AllowOldToOverwrite
        {
            get => _allowOldToOverwrite;
            set => SetProperty(ref _allowOldToOverwrite, value);
        }
        private bool _allowOldToOverwrite = false;
    }

    public abstract  class InstallationTask<T> : IProgress<float>, INotifyPropertyChanged
    {
        protected String SourceFilepath { get; }

        public abstract event InstallationTaskCompletedEventHandler InstallationTaskComplete;
        public delegate void InstallationTaskCompletedEventHandler(InstallationTask<T> source);

        public void Report(float value)
        {
            Progress = _progressRange.Item1 + value * (_progressRange.Item2 - _progressRange.Item1);
        }
        protected (float, float) _progressRange = (0, 1);

        #region NotifiableProperties
        public float Progress
        {
            get => _progress;
            set => SetProperty(ref _progress, value);
        }
        protected float _progress = 0.1f;

        public bool IsInstalling
        {
            get => _isInstalling;
            set => SetProperty(ref _isInstalling, value);
        }
        protected bool _isInstalling = false;

        #endregion

        public InstallationTask(String source_file_path)
        {
            SourceFilepath = source_file_path;
        }

        public abstract Task<T?> RunInstall();

        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler? PropertyChanged = delegate { };
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        protected void SetProperty<T>(ref T property, T value, [CallerMemberName] string propertyName = "")
        {
            property = value;
            OnPropertyChanged(propertyName);
        }
        #endregion
    }
    public partial class InstallationView : UserControl, INotifyPropertyChanged
    {
        public ObservableCollection<ModInstallationTask> Installations { get; } = new();

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

        private System.Windows.Forms.OpenFileDialog CreateOpenFileDialog()
        {
            return new System.Windows.Forms.OpenFileDialog
            {
                Filter = "Zip Archives (*.zip)|*.zip",
                RestoreDirectory = true, // TODO keep location separate from game path dialog, it's annoying!
                Multiselect = true
            };
        }

        private void OnInstallFromZip(object sender, RoutedEventArgs e)
        {
            if (ModCollection.Global is null) return;

            var dialog = CreateOpenFileDialog();
            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            IsInstalling = true;

            List<Task<ModCollection?>> InstallationTasks = new();

            foreach (var Filename in dialog.FileNames)
            {
                ModInstallationTask InstallationTask = new ModInstallationTask(Filename);
                //add to displayed list
                Installations.Add(InstallationTask);
                //add to list of tasks for parallel async
                InstallationTasks.Add(InstallationTask.RunInstall());

                //notify if extraction is finished
                InstallationTask.InstallationTaskComplete += x => Installations.Remove((ModInstallationTask)x);
            }

            _ = Task.Run( async () =>
                {
                    IEnumerable<ModCollection?> Extracted = await Task.WhenAll(InstallationTasks);

                    foreach (var collection in Extracted)
                    {
                        if (collection is not null)
                        {
                            Console.WriteLine($"Install zip: {collection.ModsPath}");

                            // TODO progress for MoveIntoAsync should be done per mod 

                            // taubes comment: I happily leave that cancer to you :D
                            await ModCollection.Global.MoveIntoAsync(collection,
                                AllowOldToOverwrite);
                        }
                    }
                }
            );

            IsInstalling = false;
            MainViewController.Instance.SetView(View.MOD_ACTIVATION);

            //nuked comments (I am sorry jakob)

            // TODO current progress assumes all zip files take similarily long
            //      this can be improved by giving absolute progress vs MB size for example
            //      but that's an update to be done when zip actually supports progress

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
