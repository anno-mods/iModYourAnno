using Imya.GithubIntegration.StaticData;
using Octokit;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.GithubIntegration
{
    public class GithubRepoInfo
    {
        public String Name { get; init; }
        public String Owner { get; init; }
        public String ReleaseID { get; init; }

        public string? CreatorName { get; init; }
        public string? ModName { get; init; }
        public string? Readme { get; init; }

        public GithubRepoInfo(string name, string owner, string releaseid)
        {
            Name = name;
            Owner = owner;
            ReleaseID = releaseid;
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is not GithubRepoInfo other) return false;
            return Name == other.Name && Owner == other.Owner && ReleaseID == other.ReleaseID;
        }

        public override int GetHashCode() => base.GetHashCode();

        public override String ToString()
        {
            return $"{Owner}/{Name} : {ReleaseID}";
        }

        public String GetID()
        {
            return $"{Owner}_{Name}_{ReleaseID}";
        }
    }

}
