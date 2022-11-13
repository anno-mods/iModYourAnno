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
        public event PopupRequestedEventHandler UserCodeReceived;
        public delegate void PopupRequestedEventHandler(String Context);

        public event AuthenticatedEventHandler AuthenticationSuccess;
        public delegate void AuthenticatedEventHandler();

        public Task StartAuthentication();
        public bool HasStoredLoginInfo();
        public void RemoveAuthentication();
    }
}