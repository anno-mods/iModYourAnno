using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.GithubIntegration
{
    public struct GithubRepoInfo
    {
        public String Name;
        public String Owner;
        public String AssetName;

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is not GithubRepoInfo other) return false;
            return Name == other.Name && Owner == other.Owner && AssetName == other.AssetName;
        }
    }
}
