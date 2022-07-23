using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octokit;

namespace Imya.GithubIntegration.RepositoryInformation
{
    internal interface IRepositoryProvider
    {
        Task<Release?> FetchLatestReleaseAsync(GithubRepoInfo repoInfo);

        Task<Repository?> FetchRepositoryAsync(GithubRepoInfo repoInfo);
    }
}
