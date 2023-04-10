using Imya.GithubIntegration.RepositoryInformation;
using Microsoft.Extensions.FileSystemGlobbing;
using Octokit;

namespace Imya.GithubIntegration.StaticData
{
    public class StaticNameReleaseAssetStrategy : IReleaseAssetStrategy
    {
        static IRepositoryProvider? _releaseProvider;        

        public StaticNameReleaseAssetStrategy(IRepositoryProvider releaseProvider)
        {
            _releaseProvider = releaseProvider;
        }

        public async Task<ReleaseAsset?> GetReleaseAssetAsync(GithubRepoInfo repoInfo)
        {
            var release = await _releaseProvider!.FetchLatestReleaseAsync(repoInfo);
            if (release is null)
                return null;

            Matcher matcher = new();
            matcher.AddIncludePatterns(new string[] { repoInfo.ReleaseID });

            return release?.Assets.FirstOrDefault(x => matcher.Match(x.Name).HasMatches);
        }
    }
}
