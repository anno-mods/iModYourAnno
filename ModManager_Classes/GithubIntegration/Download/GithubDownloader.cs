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

namespace Imya.GithubIntegration.Download
{
    public struct DownloadResult
    {
        public DownloadResult() { }

        public bool DownloadSuccessful = false;
        public String DownloadDestination = String.Empty;
    }

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
            var downloadURL = releaseAsset.BrowserDownloadUrl;
            if (downloadURL is null) throw new InstallationException("No matching release found");

            String TargetFilename = Path.Combine(Options.DownloadDirectory, releaseAsset.Name);

            try
            {
                using (HttpClient DownloadClient = new HttpClient())
                using (Stream targetStream = File.Create(TargetFilename))
                {
                    DownloadClient.Timeout = Options.Timeout;
                    await DownloadClient.DownloadAsync(downloadURL, targetStream, Options.DownloadBufferSize, Options.Timeout, progress);
                    return new DownloadResult { DownloadSuccessful = true, DownloadDestination = TargetFilename };
                }
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

    public static class HttpClientExtensions
    {
        public static async Task DownloadAsync(this HttpClient client,
            String RequestUri, 
            Stream destination,
            int BufferSize,
            TimeSpan Timeout,
            IProgress<float>? progress = null, 
            CancellationToken cancellationToken = default)
        {
            using (var response = await client.GetAsync(RequestUri, HttpCompletionOption.ResponseHeadersRead))
            using (var ContentStream = await response.Content.ReadAsStreamAsync())
            {
                var contentLength = response.Content.Headers.ContentLength ?? 0;
                await ContentStream.CopyToAsync(destination, BufferSize, Timeout, progress, cancellationToken, contentLength);
            }
        }
    }

    public static class StreamExtensions
    {
        public static async Task CopyToAsync(this Stream source, 
            Stream destination, 
            int bufferSize, 
            TimeSpan Timeout,
            IProgress<float>? progress = null, 
            CancellationToken cancellationToken = default, 
            long totalBytes = 1)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!source.CanRead)
                throw new ArgumentException("Has to be readable", nameof(source));
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));
            if (!destination.CanWrite)
                throw new ArgumentException("Has to be writable", nameof(destination));
            if (bufferSize < 0)
                throw new ArgumentOutOfRangeException(nameof(bufferSize));

            var buffer = new byte[bufferSize];
            long totalBytesRead = 0;
            int bytesRead;
            while ((bytesRead = await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken).TimeoutAfter(Timeout)) != 0)
            {
                await destination.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                totalBytesRead += bytesRead;
                progress?.Report((float)totalBytesRead / totalBytes);
            }
        }
    }
}