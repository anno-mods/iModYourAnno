using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.GithubIntegration.StaticData
{
    internal class StaticReadmeFilepathStrategy : IReadmeFilepathStrategy
    {
        public string GetMarkdownReadmeFilepath()
        {
            return "imya.md";
        }
    }
}
