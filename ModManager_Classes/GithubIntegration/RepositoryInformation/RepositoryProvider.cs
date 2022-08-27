using Imya.Utils;
using Octokit;

namespace Imya.GithubIntegration.RepositoryInformation
{
    internal class RepositoryProvider : IRepositoryProvider
    {
        private GitHubClient _githubClient = GithubClientProvider.Client;

        public RepositoryProvider()
        {

        }

        public async Task<Release?> FetchLatestReleaseAsync(GithubRepoInfo repository)
        {
            try
            {
                return await _githubClient.Repository.Release.GetLatest(repository.Owner, repository.Name);
            }
            catch (ApiException e)
            {
                
            }
            return null;
        }

        public async Task<IReadOnlyList<Release>?> FetchReleasesAsync(GithubRepoInfo repository)
        {
            try
            {
                return await _githubClient.Repository.Release.GetAll(repository.Owner, repository.Name);
            }
            catch (ApiException e)
            {

            }
            return null;
        }

        public async Task<Repository?> FetchRepositoryAsync(GithubRepoInfo repoInfo)
        {
            try
            {
                return await _githubClient.Repository.Get(repoInfo.Owner, repoInfo.Name);
            }
            catch (ApiException e)
            {
                
            }
            return null;
        }
    }
}
