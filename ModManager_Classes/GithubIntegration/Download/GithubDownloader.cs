using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Imya.GithubIntegration.RepositoryInformation;
using Imya.Models.Options;
using Imya.Utils;
using Octokit;

using Downloader;


namespace Imya.GithubIntegration.Download
{
    public struct DownloadResult
    {
        public DownloadResult() { }

        public bool DownloadSuccessful = false;
        public String DownloadDestination = String.Empty;
    }

    [Obsolete]
    public class GithubDownloader
    {
        public GithubDownloaderOptions Options = new GithubDownloaderOptions();

        public GithubDownloader(GithubDownloaderOptions _options)
        {
            Options = _options;
            if (!Directory.Exists(Options.DownloadDirectory))
            {
                Directory.CreateDirectory(Options.DownloadDirectory);
            }
        }

        /// <summary>
        /// Downloads a Release into the download folder.
        /// </summary>
        /// <param name="release">the release to be downloaded</param>
        /// <param name="AssetName">the name of the asset in the release to fetch</param>
        /// <returns>the Filepath where the release has been downloaded to.</returns>
        private async Task<DownloadResult> DownloadReleaseAssetAsync(ReleaseAsset releaseAsset, IProgress<float>? progress = null)
        {
            if (releaseAsset.BrowserDownloadUrl is null) throw new InstallationException("No matching release found");

            String TargetFilename = Path.Combine(Options.DownloadDirectory, releaseAsset.Name);

            try
            {
                //add configuration options like download limit later
                var downloadOpt = new DownloadConfiguration()
                {
                    ChunkCount = 4,
                    ParallelDownload = true,
                    TempDirectory = Options.DownloadDirectory
                };

                IDownload download = DownloadBuilder.New()
                    .WithUrl(releaseAsset.BrowserDownloadUrl)
                    .WithFileLocation(TargetFilename)
                    .WithConfiguration(downloadOpt)
                    .Build();

                if (progress is not null)
                {
                    download.DownloadProgressChanged += (sender, e) => {
                        progress.Report((float)e.ProgressPercentage / 100);
                    };
                }

                await download.StartAsync();
                return new DownloadResult { DownloadSuccessful = true, DownloadDestination = TargetFilename };
            }
            catch (Exception e)
            {
                throw new InstallationException($"Download failed: {e.Message}");
            }
        }

        public async Task<DownloadResult> DownloadRepoInfoAsync(GithubRepoInfo repoInfo, IProgress<float>? progress = null)
        {
            var releaseAsset = await repoInfo.GetReleaseAssetAsync();

            if (releaseAsset is null)
            {
                //return new DownloadResult { DownloadSuccessful = false };
                throw new InstallationException($"Could not fetch any Release for {repoInfo}");
            }

            return await DownloadReleaseAssetAsync(releaseAsset, progress);
        }
    }
}