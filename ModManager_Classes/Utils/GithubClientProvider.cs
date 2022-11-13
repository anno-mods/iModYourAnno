using Octokit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;


namespace Imya.Utils
{
    public class GithubClientProvider
    {
        public static GitHubClient Client { get; } = new GitHubClient(new ProductHeaderValue("iModYourAnno"));

        public static IAuthenticator Authenticator { get; set; }
    }
}
