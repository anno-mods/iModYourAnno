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

        public String ReleaseAssetName { get => GetReleaseAssetName(); }
        public String ReadmeMarkdownFilepath { get => GetMarkdownReadmeFilepath(); }

        private IReleaseAssetNameStrategy _releaseAssetNameStrategy;
        private IReadmeFilepathStrategy _readmeFilepathStrategy;

        public GithubRepoInfo(
            IReleaseAssetNameStrategy releaseAssetNameStrategy,
            IReadmeFilepathStrategy readmeFilepathStrategy,
            String owner, 
            String repoName)
        {
            Name = repoName;
            Owner = owner;

            _releaseAssetNameStrategy = releaseAssetNameStrategy;
            _readmeFilepathStrategy = readmeFilepathStrategy;
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is not GithubRepoInfo other) return false;
            return Name == other.Name && Owner == other.Owner && GetReleaseAssetName() == other.GetReleaseAssetName();
        }

        public String GetReleaseAssetName()
        {
            return _releaseAssetNameStrategy.GetReleaseAssetName();
        }

        public String GetMarkdownReadmeFilepath()
        { 
            return _readmeFilepathStrategy.GetMarkdownReadmeFilepath();
        }

        public override String ToString()
        {
            return $"{Owner}/{Name} : {GetReleaseAssetName()}";
        }
    }

}
