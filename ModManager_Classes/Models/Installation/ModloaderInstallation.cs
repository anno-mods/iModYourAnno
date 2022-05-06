using System.IO.Compression;
using Imya.GithubIntegration;
using Imya.Models;
using Imya.Models.Installation;

namespace Imya.Utils
{
    /// <summary>
    /// Check and install mod loader.
    /// 
    /// TODO (taubenangriff): Transform this thing into a reusable install class.
    /// </summary>
    public class ModloaderInstallation : Installation
    {
        readonly GameSetupManager GameSetup = GameSetupManager.Instance;
        readonly GithubDownloader GithubDownloader;

        public string DownloadDirectory { get => GameSetup.DownloadDirectory; } 

        public static GithubRepoInfo ModloaderRepository { get; } = new GithubRepoInfo() { Name = "anno1800-mod-loader", Owner = "xforce" };

        public new ModloaderInstallationStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }
        private ModloaderInstallationStatus _status = ModloaderInstallationStatus.NotStarted;

        public ModloaderInstallation()
        {
            GithubDownloader = new GithubDownloader(DownloadDirectory);
            if (GameSetup.GameRootPath == null)
                return;

            HeaderText = TextManager.Instance.GetText("INSTALLATION_HEADER_LOADER");

            //do we need that here?
            //GameSetup.UpdateModloaderInstallStatus();
        }

        /// <summary>
        /// Download and install mod loader from GitHub.
        /// </summary>
        public async Task InstallAsync()
        {
            if (GameSetup.ExecutableDir == null)
            {
                // TODO disable install UI based on game path setting
                Console.WriteLine($"Game path is not set yet.");
                return;
            }

            Status = ModloaderInstallationStatus.Downloading;

            var downloadResult = await GithubDownloader.DownloadReleaseAsync(ModloaderRepository, "loader.zip", progress: this);
            if (!downloadResult.DownloadSuccessful) return;

            String DownloadFilename = downloadResult.DownloadDestination;
            string target = Path.Combine(Path.GetDirectoryName(DownloadFilename) ??"", Path.GetFileNameWithoutExtension(DownloadFilename));

            Status = ModloaderInstallationStatus.Unpacking;
            ZipFile.ExtractToDirectory(DownloadFilename, target, true);

            Status = ModloaderInstallationStatus.MovingFiles;
            foreach (string absFile in Directory.GetFiles(target))
            { 
                string relFile = Path.GetFileName(absFile);
                File.Move(Path.Combine(target, relFile), Path.Combine(GameSetup.ExecutableDir, relFile), true);
            }

            Directory.Delete(target);

            GameSetup.UpdateModloaderInstallStatus();
        }

        /// <summary>
        /// Check if there's an updated mod loader on GitHub.
        /// </summary>
        public async Task CheckForUpdatesAsync()
        {
            // TODO
            // - check release tag against local version
            // - optional: crawl release notes for latest "Game Update ?\d\d(\.\d)?" or "GU ?\d\d(\.\d)?" 
            await Task.Run(() => { });
        }
    }

    public class ModloaderInstallationStatus : IInstallationStatus
    {
        public static readonly ModloaderInstallationStatus NotStarted = new("ZIP_NOTSTARTED");
        public static readonly ModloaderInstallationStatus Downloading = new("INSTALL_DOWNLOAD");
        public static readonly ModloaderInstallationStatus Unpacking = new("ZIP_UNPACKING");
        public static readonly ModloaderInstallationStatus MovingFiles = new("ZIP_MOVING");

        private readonly string _value;
        private ModloaderInstallationStatus(string value)
        {
            _value = value;
        }

        public IText Localized => TextManager.Instance[_value];
    }

}
