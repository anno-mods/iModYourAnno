using Imya.Utils;
using Octokit;

namespace Imya.GithubIntegration.RepositoryInformation
{
    public class RepositoryProvider : IRepositoryProvider
    {
        private IGitHubClient _githubClient;

        public RepositoryProvider(IGitHubClient client)
        {
            _githubClient = client; 
        }

        public async Task<Release?> FetchLatestReleaseAsync(GithubRepoInfo repository)
        {
            try
            {
                return await _githubClient.Repository.Release.GetLatest(repository.Owner, repository.Name);
            }
            catch (RateLimitExceededException e)
            {
                throw e; 
            }
            catch (ApiException e)
            { }
            return null;
        }

        public async Task<IReadOnlyList<Release>?> FetchReleasesAsync(GithubRepoInfo repository)
        {
            try
            {
                return await _githubClient.Repository.Release.GetAll(repository.Owner, repository.Name);
            }
            catch (RateLimitExceededException e)
            {
                throw e;
            }
            catch (ApiException e)
            { }
            return null;
        }

        public async Task<Repository?> FetchRepositoryAsync(GithubRepoInfo repoInfo)
        {
            try
            {
                return await _githubClient.Repository.Get(repoInfo.Owner, repoInfo.Name);
            }
            catch (RateLimitExceededException e)
            {
                throw e;
            }
            catch (ApiException e)
            { }
            return null;
        }
    }
}
