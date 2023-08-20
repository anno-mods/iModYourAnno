using Imya.Models.Collections;
using Imya.Models.Installation;

using Downloader;
using Imya.Models.NotifyPropertyChanged;
using System.Collections.ObjectModel;
using Imya.Services.Interfaces;
using Imya.Utils;
using Imya.Models.Installation.Interfaces;
using Anno.EasyMod.Mods.LocalMods;

namespace Imya.Services
{
    public class InstallationService : PropertyChangedNotifier, IInstallationService
    {
        public List<IUnpackable> Unpacks { get; private set; }

        //we want a special queue that we can manipulate later on. 
        public IQueue<IDownloadableUnpackableInstallation> PendingDownloads { get; private set; }
        public IDownloadableInstallation? CurrentDownload { get; private set; }

        public ObservableCollection<IInstallation> ActiveInstallations { get; private set; }
        public List<string> CurrentGithubInstallsIDs { get; private set; }

        public DownloadConfiguration DownloadConfig { get; private set; }

        public DownloadService DownloadService
        {
            get => _downloadService;
            set => SetProperty(ref _downloadService, value);
        }
        private DownloadService _downloadService;

        private Semaphore _moveIntoSem;
        private Semaphore _downloadSem;

        private delegate void GithubInstallAddedEventHandler(GithubInstallation githubInstallation);
        private event GithubInstallAddedEventHandler GithubInstallAdded = delegate { };

        private delegate void ZipInstallAddedEventHandler(ZipInstallation zipInstallation);
        private event ZipInstallAddedEventHandler ZipInstallAdded = delegate { };

        public event IInstallationService.InstallationCompletedEventHandler InstallationCompleted = delegate { };
        public event IInstallationService.InstallFailedEventHandler InstallFailedWithException = delegate { };

        public double BytesPerSecondSpeed
        {
            get => _bytesPerSecondSpeed;
            private set => SetProperty(ref _bytesPerSecondSpeed, value);
        }
        private double _bytesPerSecondSpeed;

        public double ProgressPercentage
        {
            get => _progressPercentage;
            private set => SetProperty(ref _progressPercentage, value);
        }
        private double _progressPercentage;

        public bool IsInstalling
        {
            get => _isInstalling;
            private set => SetProperty(ref _isInstalling, value);
        }
        private bool _isInstalling = false;

        public int TotalInstallationCount
        {
            get => _totalInstallationCount;
            private set
            {
                IsInstalling = value > 0;
                SetProperty(ref _totalInstallationCount, value);
            }
        }
        private int _totalInstallationCount;

        public int PendingInstallationsCount
        {
            get => _pendingInstallationCount;
            private set => SetProperty(ref _pendingInstallationCount, value);
        }
        private int _pendingInstallationCount;

        public int RunningInstallationsCount
        {
            get => _runningInstallationCount;
            private set => SetProperty(ref _runningInstallationCount, value);
        }
        private int _runningInstallationCount;

        public double CurrentDownloadSpeedPerSecond
        {
            get => _currentDownloadSpeedPerSecond;
            set => SetProperty(ref _currentDownloadSpeedPerSecond, value);
        }
        private double _currentDownloadSpeedPerSecond;

        #region some_constants
        private float min_progress = 0;
        private float max_dl_progress = 0.8f;
        private float max_progress = 1;
        #endregion

        private readonly ILocalModCollectionFactory _modCollectionFactory;
        private readonly IImyaSetupService _imyaSetupService;

        public InstallationService(
            ILocalModCollectionFactory factory,
            IImyaSetupService imyaSetupService)
        {
            _modCollectionFactory = factory;
            _imyaSetupService = imyaSetupService; 
            _moveIntoSem = new Semaphore(1, 1);
            _downloadSem = new Semaphore(1, 1);

            PendingDownloads = new WrappedQueue<IDownloadableUnpackableInstallation>();
            Unpacks = new List<IUnpackable>();
            ActiveInstallations = new();
            CurrentGithubInstallsIDs = new();

            DownloadConfig = new DownloadConfiguration();

            //TODO add options
            DownloadService = new(DownloadConfig);
            DownloadService.DownloadProgressChanged += OnDownloadProgressChanged;

            //when an install gets added, we invoke process with next download, semaphore does the rest for us. 
            GithubInstallAdded += async (x) => await ProceedWithNextDownloadAsync();
            ZipInstallAdded += async (x) => await ExecuteZipInstall(x);

            InstallationCompleted += () =>
            {
                TotalInstallationCount--;
                RunningInstallationsCount--;
            };
        }

        public void EnqueueGithubInstallation(GithubInstallation githubInstallation)
        {
            TotalInstallationCount++;
            PendingDownloads.Enqueue(githubInstallation);
            PendingInstallationsCount++;
            CurrentGithubInstallsIDs.Add(githubInstallation.RepositoryToInstall.GetID());
            //Signal that a github install was added.
            GithubInstallAdded?.Invoke(githubInstallation);
        }

        public void EnqueueZipInstallation(ZipInstallation zipInstallation)
        {
            TotalInstallationCount++;
            Unpacks.Add(zipInstallation);
            ZipInstallAdded?.Invoke(zipInstallation);
        }

        public bool IsProcessingInstallWithID(string id)
        {
            return CurrentGithubInstallsIDs.Any(x => x == id);
        }

        public void Pause()
        {
            if (CurrentDownload is null) 
                return;
            DownloadService.Pause();
            CurrentDownload.IsPaused = true;
            BytesPerSecondSpeed = 0;
        }

        public void Resume()
        {
            if (CurrentDownload is null)
                return;
            DownloadService.Resume();
            CurrentDownload.IsPaused = false;
        }

        public async Task CancelAsync(IInstallation installation)
        {
            installation.CancellationTokenSource.Cancel();

            if (installation is IUnpackable unpackable)
                CleanUpUnpackable(unpackable);

            if (installation is IDownloadable downloadable)
            {
                if (downloadable == CurrentDownload)
                {
                    DownloadService.CancelAsync();
                    await DownloadService.Clear();
                    RestartDownloadService();
                    CurrentDownload = null;
                }
                CleanUpDownloadable(downloadable);
            }
        }

        public void RemovePending(IDownloadableUnpackableInstallation install)
        {
            PendingDownloads.Remove(install);
            PendingInstallationsCount--;
        }

        private async Task ExecuteZipInstall(IUnpackableInstallation zipInstallation)
        {
            ActiveInstallations.Add(zipInstallation);
            await Task.Run(async () =>
            {
                await UnpackAsync(zipInstallation);
                await MoveModsAsync(zipInstallation);
                CleanUpUnpackable(zipInstallation);
            });
            ActiveInstallations.Remove(zipInstallation);
        }

        private async Task ProceedWithNextDownloadAsync()
        {
            //wait for the current download to finish;
            await Task.Run(() => _downloadSem.WaitOne());
            if (PendingDownloads.Count() == 0) return;

            //get the next download
            Console.WriteLine("Starting Download");
            var installation = PendingDownloads.Dequeue();
            PendingInstallationsCount--;
            RunningInstallationsCount++;

            ActiveInstallations.Add(installation);
            installation.CanBePaused = true;
            installation.IsBeingDownloaded = true;
            await DownloadAsync(installation);
            installation.CanBePaused = false;
            installation.IsBeingDownloaded = false;
            _downloadSem.Release();

            Console.WriteLine("Starting Unpack");
            Unpacks.Add(installation);
            await UnpackAsync(installation);

            Console.WriteLine("Moving Mods");
            await MoveModsAsync(installation);

            CleanUpUnpackable(installation);
            CleanUpDownloadable(installation);
            ActiveInstallations.Remove(installation);
            CurrentGithubInstallsIDs.RemoveAll(x => x == installation.ID);
            InstallationCompleted?.Invoke();
        }

        private async Task UnpackAsync(IUnpackableInstallation zipInstallation)
        {
            if (zipInstallation.CancellationToken.IsCancellationRequested)
                return;

            zipInstallation.Status = InstallationStatus.Unpacking;
            await Task.Run(() =>
            {
                zipInstallation.SetProgressRange(max_dl_progress, max_progress);
                if (Directory.Exists(zipInstallation.UnpackTargetPath))
                    Directory.Delete(zipInstallation.UnpackTargetPath, true);
                using (FileStream fs = File.OpenRead(zipInstallation.SourceFilepath))
                    fs.ExtractZipFile(zipInstallation.UnpackTargetPath, overwrite: true, progress: zipInstallation);
            }, zipInstallation.CancellationToken);
        }

        private async Task MoveModsAsync(IUnpackableInstallation unpackable)
        {
            if (unpackable.CancellationToken.IsCancellationRequested)
                return;
            unpackable.Status = InstallationStatus.MovingFiles;
            var newCollection = await _modCollectionFactory.GetAsync(unpackable.UnpackTargetPath);
            //async waiting
            await Task.Run(() => _moveIntoSem.WaitOne(), unpackable.CancellationToken);
            if (!unpackable.CancellationToken.IsCancellationRequested)
            {
                await _imyaSetupService.GlobalModCollection.AddAsync(newCollection.Mods);
            }
            _moveIntoSem.Release();
            Unpacks.Remove(unpackable);
        }

        private async Task DownloadAsync(IDownloadableInstallation downloadable)
        {
            if (downloadable.CancellationToken.IsCancellationRequested)
                return;

            CurrentDownload = downloadable;
            //register progress tracking and update status
            downloadable.Status = InstallationStatus.Downloading;
            downloadable.SetProgressRange(min_progress, max_dl_progress);
            EventHandler<DownloadProgressChangedEventArgs> eventHandler = (sender, e) =>
            {
                downloadable.Report((float)e.ProgressPercentage / 100);
            };
            DownloadService.DownloadProgressChanged += eventHandler;
            //do the download
            await DownloadService.DownloadFileTaskAsync(downloadable.DownloadUrl, downloadable.DownloadTargetFilename, downloadable.CancellationToken);
            //unload the download
            DownloadService.DownloadProgressChanged -= eventHandler;
            CurrentDownload = null;
        }

        private void CleanUpUnpackable(IUnpackable unpackable)
        {
            if (Directory.Exists(unpackable.UnpackTargetPath))
                Directory.Delete(unpackable.UnpackTargetPath, true);
        }

        private void CleanUpDownloadable(IDownloadable downloadable)
        {
            if (File.Exists(downloadable.DownloadTargetFilename))
                File.Delete(downloadable.DownloadTargetFilename);
        }

        private void OnDownloadProgressChanged(object? sender, DownloadProgressChangedEventArgs e)
        {
            if (DownloadService.IsPaused)
                return;
            BytesPerSecondSpeed = e.BytesPerSecondSpeed;
            ProgressPercentage = e.ProgressPercentage;
        }

        private void RestartDownloadService()
        {
            if (DownloadService.IsBusy)
                throw new InvalidOperationException("Don't interrupt me asshole");

            DownloadService.DownloadProgressChanged -= OnDownloadProgressChanged;
            DownloadService = new(DownloadConfig);
            DownloadService.DownloadProgressChanged += OnDownloadProgressChanged;
        }
    }
}
