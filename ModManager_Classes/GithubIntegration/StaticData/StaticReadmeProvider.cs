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
                return await cache.GetOrCreateAsync(repoInfo, _ => ReadmeFunc(repoInfo));
            }
            catch (ApiException e)
            {
                return null;
            }
        }

        private async Task<String> ReadmeFunc(GithubRepoInfo repoInfo)
        {
            var readme = await GitHubClient.Repository.Content.GetAllContents(repoInfo.Owner, repoInfo.Name, repoInfo.GetMarkdownReadmeFilepath());
            var content = readme.FirstOrDefault();
            if (content is null) return String.Empty;
            return content!.Content;
        }
    }
}
