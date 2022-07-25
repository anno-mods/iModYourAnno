using Imya.GithubIntegration.StaticData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.GithubIntegration.StaticData
{
    public class StaticRepoInfoSource : IRepoInfoSource
    {
        private Random random = new Random();

        private GithubRepoInfo[] Repositories = {
            StaticNameGithubRepoInfoFactory.CreateWithStaticName("Spice-it-Up", "anno-mods", "Spice-it-Up.zip"),
            StaticNameGithubRepoInfoFactory.CreateWithStaticName("New-World-Tourism", "anno-mods", "Gameplay.New.World.Tourism.zip"),
            StaticNameGithubRepoInfoFactory.CreateWithStaticName("anno-1800-jakobs-mods", "jakobharder", "Gameplay.Modular.Factories.zip"),
            StaticNameGithubRepoInfoFactory.CreateWithStaticName("anno-1800-jakobs-mods", "jakobharder", "Jakobs-Collection-v4.11.zip"),
            StaticNameGithubRepoInfoFactory.CreateWithStaticName("anno-1800-jakobs-mods", "jakobharder", "Misc.Production.Variations.zip"),
            StaticNameGithubRepoInfoFactory.CreateWithStaticName("anno-1800-jakobs-mods", "jakobharder", "Shared.Ground.Textures.zip"),
            StaticNameGithubRepoInfoFactory.CreateWithStaticName("WholesomeHaciendaHaul", "Taludas", "TheWholesomeHaciendaHaul_v1.3.2.zip"),
            StaticNameGithubRepoInfoFactory.CreateWithStaticName("SmallModsCollection", "Taludas", "SmallModsCollection_v1.0.zip"),
            //StaticNameGithubRepoInfoFactory.CreateWithStaticName("Police_Station_MU", "muggenstuermer", "Source Code"),
            //StaticNameGithubRepoInfoFactory.CreateWithStaticName("Tourists_Alternativ_Buildings_MU", "muggenstuermer", "Source Code"),
            //StaticNameGithubRepoInfoFactory.CreateWithStaticName("Hospital_MU", "muggenstuermer", "Source Code"),
            //StaticNameGithubRepoInfoFactory.CreateWithStaticName("Fire_Station_MU", "muggenstuermer", "Source Code"),
            //StaticNameGithubRepoInfoFactory.CreateWithStaticName("MuggenTours_MU", "muggenstuermer", "Source Code"),
            //StaticNameGithubRepoInfoFactory.CreateWithStaticName("City_Ornaments_MU_2.0", "muggenstuermer", "Source Code"),
            //StaticNameGithubRepoInfoFactory.CreateWithStaticName("thesacredjeditexts", "luke skywalker", "Source Code"),
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
            return Repositories[random.NextInt64(0,Repositories.LongLength)];
        }
    }
}
