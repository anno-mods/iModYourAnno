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

        private IReleaseAssetNameStrategy _strategy;

        public GithubRepoInfo(IReleaseAssetNameStrategy strat)
        {
            _strategy = strat;
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is not GithubRepoInfo other) return false;
            return Name == other.Name && Owner == other.Owner && GetReleaseAssetName() == other.GetReleaseAssetName();
        }

        public String GetReleaseAssetName()
        {
            return _strategy.GetReleaseAssetName();
        }

        public override String ToString()
        {
            return $"{Owner}/{Name} : {GetReleaseAssetName()}";
        }
    }

}
