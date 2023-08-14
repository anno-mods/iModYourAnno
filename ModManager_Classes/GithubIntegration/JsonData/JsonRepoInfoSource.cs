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
                //public string? name; // unused and not defined
                public string? repo;
                public string? owner;
                public string? id;
                public string? creatorName;
                public string? modName;
                public string? readme;

                // optional
                public string? download;
            }

            public RepoIndexEntry[]? packages;
        }
#pragma warning restore 0649

        public static readonly JsonRepoInfoSource Empty = new();
        private IEnumerable<GithubRepoInfo> repositories = Array.Empty<GithubRepoInfo>();

        public JsonRepoInfoSource(string json)
        {
            _ = Parse(json);
        }

        protected JsonRepoInfoSource()
        {
            // empty source
        }

        protected bool Parse(string json)
        {
            RepoIndex index;
            try
            {
                index = JsonConvert.DeserializeObject<RepoIndex>(json, new JsonSerializerSettings()
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore
                });
            }
            catch
            {
                index.packages = null;
            }

            if (index.packages is null)
            {
                return false;
            }

            // TODO static is hardcoded now, but shouldn't as soon as we support the others
            var packages = index.packages.Where(x => x.repo is not null && x.owner is not null && x.download is not null);
            repositories = packages.Select(x => new GithubRepoInfo(x.repo!, x.owner!, x.download!)
            {
                CreatorName = x.creatorName,
                ModName = x.modName,
                Readme = x.readme,
            });
            return true;
        }

        private readonly Random random = new();

        public IEnumerable<GithubRepoInfo> Get(int count) => repositories.Take(count);
        public IEnumerable<GithubRepoInfo> GetAll() => repositories;
        public GithubRepoInfo GetSingle() => repositories.ElementAt((int)random.NextInt64(0, repositories.Count()));
    }
}
