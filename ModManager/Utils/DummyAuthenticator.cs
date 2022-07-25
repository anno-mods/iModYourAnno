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
        public event IAuthenticator.PopupRequestedEventHandler UserCodeReceived = delegate { };
        public event IAuthenticator.AuthenticatedEventHandler AuthenticationSuccess = delegate { };

        public async Task RunAuthenticate(GitHubClient Client)
        {
            Console.WriteLine("Authentication is not supported currently!");
        }
    }
}