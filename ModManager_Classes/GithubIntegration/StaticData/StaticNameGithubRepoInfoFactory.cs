using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.GithubIntegration.StaticData
{
    public class StaticNameGithubRepoInfoFactory
    {
        public static GithubRepoInfo CreateWithStaticName(String RepositoryName, String Owner, String AssetName)
        {
            IReleaseAssetStrategy release_asset_strat = new StaticNameReleaseAssetStrategy(AssetName);
            IReadmeFilepathStrategy filepath_strat = new StaticReadmeFilepathStrategy();

            return new GithubRepoInfo(release_asset_strat, filepath_strat, Owner, RepositoryName, AssetName);
        }
    }
}
