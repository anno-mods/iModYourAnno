using Imya.GithubIntegration.StaticData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Imya.GithubIntegration.JsonData
{
    public class JsonRepoInfoSource : IRepoInfoSource
    {
#pragma warning disable 0649
        private struct RepoIndex
        {
            public struct RepoIndexEntry
            {
                public string? name;
                public string? repo;
                public string? owner;
                public string? id;

                // optional
                public string? download;
            }

            public RepoIndexEntry[]? packages;
        }
#pragma warning restore 0649

        private readonly IEnumerable<GithubRepoInfo> repositories;

        public JsonRepoInfoSource(string jsonFile)
        {
            var index = JsonConvert.DeserializeObject<RepoIndex>(jsonFile, new JsonSerializerSettings()
            {
                MissingMemberHandling = MissingMemberHandling.Ignore
            });

            if (index.packages is null)
            {
                repositories = Array.Empty<GithubRepoInfo>();
                return;
            }

            // TODO static is hardcoded now, but shouldn't as soon as we support the others
            var packages = index.packages.Where(x => x.repo is not null && x.owner is not null && x.download is not null);
            repositories = packages.Select(x =>
                StaticNameGithubRepoInfoFactory.CreateWithStaticName(x.repo!, x.owner!, x.download!));
        }

        private readonly Random random = new();

        public IEnumerable<GithubRepoInfo> Get(int count) => repositories.Take(count);
        public IEnumerable<GithubRepoInfo> GetAll() => repositories;
        public GithubRepoInfo GetSingle() => repositories.ElementAt((int)random.NextInt64(0, repositories.Count()));
    }
}
