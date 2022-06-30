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
            StaticNameReleaseAssetStrategy strat = new(AssetName);
            return new GithubRepoInfo(strat) { Owner = Owner, Name = RepositoryName };
        }
    }
}
