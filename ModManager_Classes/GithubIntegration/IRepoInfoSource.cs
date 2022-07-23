using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.GithubIntegration
{
    public interface IRepoInfoSource
    {
        public IEnumerable<GithubRepoInfo> GetAll();
        public IEnumerable<GithubRepoInfo> Get(int count);

        public GithubRepoInfo GetSingle();
    }
}
