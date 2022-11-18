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
        public bool IsAuthenticated { get; set; }
        public String? AuthenticatedUser { get; set; }

        public event IAuthenticator.PopupRequestedEventHandler UserCodeReceived = delegate { };
        public event IAuthenticator.AuthenticatedEventHandler AuthenticationSuccess = delegate { };

        public bool HasStoredLoginInfo()
        {
            throw new NotImplementedException();
        }

        public void RemoveAuthentication()
        {
            throw new NotImplementedException();
        }

        public async Task StartAuthentication()
        {
            Console.WriteLine("Authentication is not supported currently!");
        }
    }
}