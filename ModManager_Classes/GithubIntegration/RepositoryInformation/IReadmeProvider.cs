using Octokit;

namespace Imya.GithubIntegration.RepositoryInformation
{
    internal interface IReadmeProvider
    {
        public Task<String?> GetReadmeAsync(GithubRepoInfo repoInfo);
    }
}
