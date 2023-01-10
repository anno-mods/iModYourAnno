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

        public String ReadmeMarkdownFilepath { get => GetMarkdownReadmeFilepath(); }

        private IReleaseAssetStrategy _releaseAssetStrategy;
        private IReadmeFilepathStrategy _readmeFilepathStrategy;
        private IModImageStrategy _imageStrategy;

        public GithubRepoInfo(
            IReleaseAssetStrategy releaseAssetStrategy,
            IReadmeFilepathStrategy readmeFilepathStrategy,
            IModImageStrategy modImageStrategy,
            String owner, 
            String repoName,
            String releaseID)
        {
            Name = repoName;
            Owner = owner;
            ReleaseID = releaseID;

            _releaseAssetStrategy = releaseAssetStrategy;
            _readmeFilepathStrategy = readmeFilepathStrategy;
            _imageStrategy = modImageStrategy;
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is not GithubRepoInfo other) return false;
            return Name == other.Name && Owner == other.Owner && ReleaseID == other.ReleaseID;
        }

        public async Task<ReleaseAsset?> GetReleaseAssetAsync() => await _releaseAssetStrategy.GetReleaseAssetAsync(this);

        public String GetMarkdownReadmeFilepath() => _readmeFilepathStrategy.GetMarkdownReadmeFilepath();

        public async Task<string?> GetImageUrlAsync() => await _imageStrategy.GetImageUrlAsync(this);

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
