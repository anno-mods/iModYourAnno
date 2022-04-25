using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Octokit;

namespace Imya.GithubIntegration
{
    public struct DownloadResult
    {
        public DownloadResult() { }

        public bool DownloadSuccessful = false;
        public String DownloadDestination = String.Empty;
    }

    public class GithubDownloader
    {
        private readonly String DOWNLOAD_DIRECTORY;

        private static GitHubClient GithubClient = new GitHubClient(new ProductHeaderValue("iModYourAnno"));

        public static int DownloadBufferSize { get; } = 81920;

        public GithubDownloader(String download_dir)
        {
            DOWNLOAD_DIRECTORY = download_dir;
            if (!Directory.Exists(DOWNLOAD_DIRECTORY))
            {
                Directory.CreateDirectory(DOWNLOAD_DIRECTORY);
            }
        }

        public async Task<Release?> FetchLatestReleaseAsync(GithubRepoInfo repository)
        {
            Release? release = null;
            try
            {
                release = await GithubClient.Repository.Release.GetLatest(repository.Owner, repository.Name);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not fetch Repository Release on {repository.Owner}/{repository.Name}: {e.Message}");
            }
            return release;
        }

        /// <summary>
        /// Downloads a Release into the download folder.
        /// </summary>
        /// <param name="release">the release to be downloaded</param>
        /// <param name="AssetName">the name of the asset in the release to fetch</param>
        /// <returns>the Filepath where the release has been downloaded to.</returns>
        public async Task<DownloadResult> DownloadReleaseAsync(Release release, String AssetName, IProgress<float>? progress = null)
        {
            var downloadURL = release.Assets.First(x => x.Name.Equals(AssetName)).BrowserDownloadUrl;
            String TargetFilename = Path.Combine(DOWNLOAD_DIRECTORY, AssetName);

            using (var DownloadClient = new HttpClient())
            using (Stream targetStream = File.Create(TargetFilename))
            {
                await DownloadClient.DownloadAsync(downloadURL, targetStream, progress, DownloadBufferSize);
            }
            return new DownloadResult{ DownloadSuccessful = true, DownloadDestination = TargetFilename };
        }

        public async Task<DownloadResult> DownloadReleaseAsync(GithubRepoInfo mod, String AssetName, IProgress<float>? progress = null)
        {
            var rel = await FetchLatestReleaseAsync(mod);
            if (rel is null) return new DownloadResult { DownloadSuccessful = false };

            return await DownloadReleaseAsync(rel, AssetName, progress);
        }
    }

    public static class HttpClientExtensions
    {
        public static async Task DownloadAsync(this HttpClient client, String RequestUri, Stream destination, IProgress<float>? progress = null, int BufferSize = 81920, CancellationToken cancellationToken = default)
        {
            using (var response = await client.GetAsync(RequestUri, HttpCompletionOption.ResponseHeadersRead))
            using (var ContentStream = await response.Content.ReadAsStreamAsync())
            {
                var contentLength = response.Content.Headers.ContentLength ?? 0;

                // Convert absolute progress (bytes downloaded) into relative progress (0% - 100%)
                // Use extension method to report progress while downloading
                await ContentStream.CopyToAsync(destination, BufferSize, progress, cancellationToken, contentLength);
            }
        }
    }

    public static class StreamExtensions
    {
        public static async Task CopyToAsync(this Stream source, Stream destination, int bufferSize, IProgress<float>? progress = null, CancellationToken cancellationToken = default, long totalBytes = 1)
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
            while ((bytesRead = await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) != 0)
            {
                await destination.WriteAsync(buffer, 0, bytesRead, cancellationToken).ConfigureAwait(false);
                totalBytesRead += bytesRead;
                progress?.Report((float)totalBytesRead / totalBytes);
            }
        }
    }
}