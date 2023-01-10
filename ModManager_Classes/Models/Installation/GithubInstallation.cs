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
    public class GithubInstallation : Installation, IDownloadableUnpackableInstallation
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
}
