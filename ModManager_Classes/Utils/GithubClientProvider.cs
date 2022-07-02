using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Utils
{
    internal class GithubClientProvider
    {
        internal static GitHubClient Client { get; } = new GitHubClient(new ProductHeaderValue("iModYourAnno"));
    }
}
