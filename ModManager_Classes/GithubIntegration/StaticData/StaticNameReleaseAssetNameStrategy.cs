using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.GithubIntegration.StaticData
{
    public class StaticNameReleaseAssetStrategy : IReleaseAssetNameStrategy
    {
        String StaticAssetName { get; init; }

        public StaticNameReleaseAssetStrategy(String Name)
        {
            StaticAssetName = Name;
        }

        public String GetReleaseAssetName()
        {
            return StaticAssetName;
        }
    }
}
