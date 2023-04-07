using Octokit;

namespace Imya.GithubIntegration.RepositoryInformation
{
    public interface IReadmeProvider
    {
        public Task<String?> GetReadmeAsync(GithubRepoInfo repoInfo);
    }
}
