using Imya.GithubIntegration.RepositoryInformation;
using Microsoft.Extensions.FileSystemGlobbing;
using Octokit;

namespace Imya.GithubIntegration.StaticData
{
    public class StaticNameReleaseAssetStrategy : IReleaseAssetStrategy
    {
        private string DownloadPattern { get; init; }

        static IRepositoryProvider? releaseProvider;        

        public StaticNameReleaseAssetStrategy(string downloadPattern)
        {
            releaseProvider ??= new RepositoryProvider();
            DownloadPattern = downloadPattern;
        }

        public async Task<ReleaseAsset?> GetReleaseAssetAsync(GithubRepoInfo repoInfo)
        {
            var release = await releaseProvider!.FetchLatestReleaseAsync(repoInfo);
            if (release is null)
                return null;

            Matcher matcher = new();
            matcher.AddIncludePatterns(new string[] { DownloadPattern });

            return release?.Assets.FirstOrDefault(x => matcher.Match(x.Name).HasMatches);
        }
    }
}
