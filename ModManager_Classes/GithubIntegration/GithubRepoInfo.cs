using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Imya.GithubIntegration
{
    public class GithubRepoInfo
    {
        public String Name { get; init; }
        public String Owner { get; init; }
        public String ReleaseID { get; init; }
        private string _releaseName;

        public string? CreatorName { get; init; }
        public string? ModName { get; init; }
        public string? Readme { get; init; }

        public string DisplayCreator => CreatorName ?? Owner;
        public string DisplayName => ModName ?? _releaseName;

        public GithubRepoInfo(string name, string owner, string releaseid)
        {
            Name = name;
            Owner = owner;
            ReleaseID = releaseid;

            _releaseName = Regex.Replace(Regex.Replace(ReleaseID, @"(\.zip|v\*|\*)", ""), @"[_.-]", " ").Trim();
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
