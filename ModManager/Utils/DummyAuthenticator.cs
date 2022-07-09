using Imya.Utils;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.UI.Utils
{
    internal class DummyAuthenticator : IAuthenticator
    {
        public async Task RunAuthenticate(GitHubClient Client)
        {
            Console.WriteLine("Authentication is not supported currently!");
        }

        public IAuthenticator.Secrets LoadSecrets(String Filename)
        {
            Console.WriteLine("Authentication is not supported currently!");
            return new IAuthenticator.Secrets();
        }
    }
}
