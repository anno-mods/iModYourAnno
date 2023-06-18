using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.GithubIntegration
{
    [Obsolete]
    public interface IReadmeFilepathStrategy
    {
        String GetMarkdownReadmeFilepath();
    }
}
