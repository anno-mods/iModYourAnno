using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.GithubIntegration
{
    public class StaticRepositoryInfoProvider : IRepositoryInfoProvider
    {
        private Random random = new Random();

        private GithubRepoInfo[] Repositories = {
            new GithubRepoInfo { Name="Spice-it-Up", Owner = "anno-mods", AssetName = "Spice-it-Up.zip"},
            new GithubRepoInfo { Name="New-World-Tourism", Owner="anno-mods", AssetName = "Gameplay.New.World.Tourism.zip" },
            new GithubRepoInfo { Name="anno-1800-jakobs-mods", Owner="jakobharder", AssetName = "Gameplay.Modular.Factories.zip" },
            new GithubRepoInfo { Name="anno-1800-jakobs-mods", Owner="jakobharder", AssetName = "Jakobs-Collection-v4.10.1.zip"},
            new GithubRepoInfo { Name="WholesomeHaciendaHaul", Owner="Taludas", AssetName = "TheWholesomeHaciendaHaul_v1.3.2.zip"},
            new GithubRepoInfo { Name="SmallModsCollection", Owner="Taludas", AssetName = "SmallModsCollection_v1.0.zip"},
            new GithubRepoInfo { Name="Police_Station_MU", Owner="muggenstuermer", AssetName = "Source Code"},
            
            //will fail all the time
            new GithubRepoInfo { Name="thesacredjeditexts", Owner="luke skywalker", AssetName = "Source Code"}
        };

        public IEnumerable<GithubRepoInfo> Get(int count)
        {
            for (int i = 0; i < count && i < Repositories.Length; i++)
            { 
                yield return Repositories[i];
            }
        }

        public IEnumerable<GithubRepoInfo> GetAll()
        {
            return Repositories;
        }

        public GithubRepoInfo GetSingle()
        {
            return Repositories[random.NextInt64(0,Repositories.LongLength - 1)];
        }
    }
}
