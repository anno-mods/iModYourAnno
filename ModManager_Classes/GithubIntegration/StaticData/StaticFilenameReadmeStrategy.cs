using Imya.Models.Cache;
using Imya.Utils;
using Microsoft.Extensions.Caching.Memory;
using Octokit;

namespace Imya.GithubIntegration.StaticData
{
    public class StaticFilenameReadmeStrategy : IReadmeStrategy
    {
        private static String _desiredFilename = "imya.md";

        private IGitHubClient _client;
        private ICache<GithubRepoInfo, String> _cache;

        public StaticFilenameReadmeStrategy(
            IGitHubClient client, 
            ICache<GithubRepoInfo, String> cache)
        {
            _client = client;
            _cache = cache; 
        }

        public async Task<String?> GetReadmeAsync(GithubRepoInfo repoInfo)
        {
            try
            {
                return await _cache.GetOrCreateAsync(repoInfo, _ => ReadmeFunc(repoInfo));
            }
            catch (RateLimitExceededException e)
            {
                throw e;
            }
            catch (ApiException e)
            {
                return null; 
            }
            
        }

        private async Task<String> ReadmeFunc(GithubRepoInfo repoInfo)
        {
            var readme = await _client.Repository.Content.GetAllContents(repoInfo.Owner, repoInfo.Name, _desiredFilename);
            var content = readme.FirstOrDefault();
            if (content is null) return String.Empty;
            return content!.Content;
        }
    }
}
