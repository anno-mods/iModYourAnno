using Downloader;
using Imya.Models.Collections;
using Imya.Models.Installation;
using Imya.Models.Installation.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Services.Interfaces
{
    public interface IInstallationService
    {
        public delegate void InstallationCompletedEventHandler();
        public event InstallationCompletedEventHandler InstallationCompleted;

        public delegate void InstallFailedEventHandler(Exception exception_context);
        public event InstallFailedEventHandler InstallFailedWithException;

        public List<IUnpackable> Unpacks { get; }
        public IQueue<IDownloadableUnpackableInstallation> PendingDownloads { get; }
        public IDownloadableInstallation? CurrentDownload { get; }

        public ObservableCollection<IInstallation> ActiveInstallations { get; }
        public List<String> CurrentGithubInstallsIDs { get; }
        public DownloadConfiguration DownloadConfig { get; }
        public DownloadService DownloadService { get; set; }

        public double BytesPerSecondSpeed { get; }
        public double ProgressPercentage { get; }
        public bool IsInstalling { get; }
        public int TotalInstallationCount { get; }
        public int PendingInstallationsCount { get; }
        public int RunningInstallationsCount { get; }

        public double CurrentDownloadSpeedPerSecond { get; }

        void EnqueueGithubInstallation(GithubInstallation githubInstallation);
        void EnqueueZipInstallation(ZipInstallation zipInstallation);
        bool IsProcessingInstallWithID(String id);
        void Pause();
        void Resume();
        Task CancelAsync(IInstallation installation);
        void RemovePending(IDownloadableUnpackableInstallation installation);
    }
}
