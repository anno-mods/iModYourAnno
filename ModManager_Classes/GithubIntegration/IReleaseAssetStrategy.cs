using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.GithubIntegration
{
    public interface IReleaseAssetStrategy
    {
        Task<ReleaseAsset?> GetReleaseAssetAsync(GithubRepoInfo repoInfo);
    }
}
