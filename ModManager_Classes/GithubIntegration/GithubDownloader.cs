using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Octokit;

namespace Imya.GithubIntegration
{
    public class GithubDownloader
    {
        private readonly String DOWNLOAD_DIRECTORY;

        private static GitHubClient GithubClient = new GitHubClient(new ProductHeaderValue("iModYourAnno"));

        public GithubDownloader(String download_dir)
        {
            DOWNLOAD_DIRECTORY = download_dir;
            if (!Directory.Exists(DOWNLOAD_DIRECTORY))
            {
                Directory.CreateDirectory(DOWNLOAD_DIRECTORY);
            }
        }

        public async Task<Release?> FetchLatestRelease(GithubRepoInfo repository)
        {
            Release? release = null;
            try
            {
                release = await GithubClient.Repository.Release.GetLatest(repository.Owner, repository.Name).ConfigureAwait(false);
                Console.WriteLine("Successfully fetched release");
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
        public async Task<String> DownloadReleaseAsync(Release release, String AssetName)
        {
            var downloadURL = release.Assets.First(x => x.Name.Equals(AssetName)).BrowserDownloadUrl;

            String TargetFilename = Path.Combine(DOWNLOAD_DIRECTORY, AssetName);

            using (var DownloadClient = new HttpClient())
            {
                var response = await DownloadClient.GetAsync(downloadURL, HttpCompletionOption.ResponseHeadersRead);

                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content;

                    //not cool tbh. Currently this is all a fucking memstream. Good Luck downloading new horizons with that.
                    using (var ContentStream = await content.ReadAsStreamAsync())
                    {
                        using (Stream targetStream = File.Create(TargetFilename))
                        {
                            await ContentStream.CopyToAsync(targetStream);
                        }
                    }
                }
            }
            return TargetFilename;
        }

        public async Task<String> DownloadReleaseAsync(GithubRepoInfo mod, String AssetName)
        {
            var FetchTask = Task.Run(async () => { return await FetchLatestRelease(mod).ConfigureAwait(false); });
            var rel = FetchTask.Result;

            var DownloadTask = Task.Run(async () => await DownloadReleaseAsync(rel, AssetName).ConfigureAwait(false));
            return DownloadTask.Result;
        }
    }
}