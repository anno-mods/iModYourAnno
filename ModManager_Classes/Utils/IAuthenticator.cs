using Newtonsoft.Json;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Utils
{
    public interface IAuthenticator
    {
        public Task RunAuthenticate(GitHubClient Client);
    }
}