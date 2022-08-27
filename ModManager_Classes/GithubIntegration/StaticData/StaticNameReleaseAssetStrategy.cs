using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imya.GithubIntegration.RepositoryInformation;
using Imya.Utils;
using Octokit;

namespace Imya.GithubIntegration.StaticData
{
    public class StaticNameReleaseAssetStrategy : IReleaseAssetStrategy
    {
        String StaticAssetName { get; init; }

        static IRepositoryProvider? releaseProvider;        

        public StaticNameReleaseAssetStrategy(String Name)
        {
            releaseProvider ??= new RepositoryProvider();
            StaticAssetName = Name;
        }

        public async Task<ReleaseAsset?> GetReleaseAssetAsync(GithubRepoInfo repoInfo)
        {
            var release = await releaseProvider!.FetchLatestReleaseAsync(repoInfo);
            return release?.Assets.FirstOrDefault(x => x.Name.Equals(StaticAssetName));
        }
    }
}
