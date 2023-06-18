using System.ComponentModel;
using System.IO.Compression;
using Imya.GithubIntegration;
using Imya.GithubIntegration.Download;
using Imya.Models;
using Imya.Models.Installation.Interfaces;
using Imya.Models.Options;
using Imya.Utils;
using Octokit;

namespace Imya.Models.Installation
{
    public class GithubInstallation : Installation, IDownloadableUnpackableInstallation,
        IPausable
    {
        public GithubRepoInfo RepositoryToInstall { get; init; }

        public string DownloadTargetFilename { get; init; }
        public string DownloadUrl { get; init; }
        public string SourceFilepath { get; init; }
        public string UnpackTargetPath { get; init; }

        public bool UseModloaderInstallFlow { get; set; }
        public long? DownloadSize { get; init; }

        public bool CanBePaused {
            get => _canBePaused;
            set => SetProperty(ref _canBePaused, value);
        }
        private bool _canBePaused;
        public bool IsPaused {
            get => _isPaused;
            set => SetProperty(ref _isPaused, value);
        }
        private bool _isPaused;

        public bool IsBeingDownloaded {
            get => _isBeingDownloaded;
            set => SetProperty(ref _isBeingDownloaded, value);
        }
        private bool _isBeingDownloaded = false;

        public GithubInstallation() 
        { }
    }
}
