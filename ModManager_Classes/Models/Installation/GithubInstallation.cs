using System.IO.Compression;
using Imya.GithubIntegration;
using Imya.GithubIntegration.Download;
using Imya.Models;
using Imya.Models.Installation;
using Imya.Models.Options;
using Imya.Utils;
using Octokit;

namespace Imya.Models.Installation
{
    public class GithubInstallation : Installation, IInstallation
    {
        public GithubRepoInfo RepositoryToInstall { get; init; }

        public string DownloadTargetFilename { get; init; }
        public string DownloadUrl { get; init; }
        public string SourceFilepath { get; init; }
        public string UnpackTargetPath { get; init; }

        public bool UseModloaderInstallFlow { get; set; }

        public long? DownloadSize { get; init; }

        public GithubInstallation() 
        {
            HeaderText = TextManager.Instance.GetText("INSTALLATION_HEADER_LOADER");
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
