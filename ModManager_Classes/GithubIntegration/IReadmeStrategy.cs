using Octokit;

namespace Imya.GithubIntegration
{
    public interface IReadmeStrategy
    {
        public Task<string?> GetReadmeAsync(GithubRepoInfo repoInfo);
    }
}
