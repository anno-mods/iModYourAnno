using Imya.GithubIntegration.RepositoryInformation;
using Imya.Models.Cache;
using Imya.Utils;
using Microsoft.Extensions.Caching.Memory;
using Octokit;

namespace Imya.GithubIntegration.StaticData
{
    public class StaticReadmeProvider : IReadmeProvider
    {
        public GitHubClient GitHubClient = GithubClientProvider.Client;
        private ICache<GithubRepoInfo, String> cache = new TimedCache<GithubRepoInfo, String>();

        public async Task<String?> GetReadmeAsync(GithubRepoInfo repoInfo)
        {
            try
            {
                return await cache.GetOrCreateAsync(repoInfo, _ => ReadmeFunc(repoInfo.Name, repoInfo.Owner));
            }
            catch (ApiException e)
            {
                return null;
            }
        }

        private async Task<String> ReadmeFunc(String Name, String Owner)
        {
            var readme = await GitHubClient.Repository.Content.GetReadme(Owner, Name);
            return readme.Content;
        }
    }
}
