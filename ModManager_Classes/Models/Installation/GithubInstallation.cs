using System.IO.Compression;
using Imya.GithubIntegration;
using Imya.Models;
using Imya.Models.Installation;
using Imya.Models.Options;
using Imya.Utils;
using Octokit;

namespace Imya.Models.Installation
{
    public abstract class GithubInstallation : Installation
    {
        protected readonly GameSetupManager GameSetup = GameSetupManager.Instance;
        protected readonly GithubDownloader GithubDownloader;

        protected String? TargetFilename = null;
        protected DownloadResult DownloadResult;

        public GithubRepoInfo RepositoryToInstall { get; private set; }

        public new GithubInstallationStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }
        private GithubInstallationStatus _status = GithubInstallationStatus.NotStarted;

        public GithubDownloaderOptions DownloaderOptions { get; set; }

        public GithubInstallation(GithubRepoInfo repoInfo, GithubDownloaderOptions? options = null)
        {
            DownloaderOptions = options ?? new GithubDownloaderOptions();
            RepositoryToInstall = repoInfo;

            GithubDownloader = new GithubDownloader(DownloaderOptions);
            if (GameSetup.GameRootPath == null)
                return;

            HeaderText = TextManager.Instance.GetText("INSTALLATION_HEADER_LOADER");
        }

        /// <summary>
        /// Download and install mod loader from GitHub.
        /// </summary>
        protected async Task DownloadAsync()
        {
            if (GameSetup.ExecutableDir == null)
            {
                // TODO disable install UI based on game path setting
                Console.WriteLine($"Game path is not set yet.");
                return;
            }
            Status = GithubInstallationStatus.Downloading;
            DownloadResult = await GithubDownloader.DownloadRepoInfoAsync(RepositoryToInstall, progress: this);
        }
    }

    public class GithubInstallationStatus : IInstallationStatus
    {
        public static readonly GithubInstallationStatus NotStarted = new("ZIP_NOTSTARTED");
        public static readonly GithubInstallationStatus Downloading = new("INSTALL_DOWNLOAD");
        public static readonly GithubInstallationStatus Unpacking = new("ZIP_UNPACKING");
        public static readonly GithubInstallationStatus MovingFiles = new("ZIP_MOVING");

        private readonly string _value;
        private GithubInstallationStatus(string value)
        {
            _value = value;
        }

        public IText Localized => TextManager.Instance[_value];
    }
}
