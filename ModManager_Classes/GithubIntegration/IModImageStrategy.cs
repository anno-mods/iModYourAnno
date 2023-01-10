using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.GithubIntegration
{
    public interface IModImageStrategy
    {
        Task<string?> GetImageUrlAsync(GithubRepoInfo repoInfo);
    }
}
