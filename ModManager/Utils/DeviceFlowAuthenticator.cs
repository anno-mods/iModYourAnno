using Imya.Models.NotifyPropertyChanged;
using Imya.Utils;
using Octokit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.UI.Utils
{
    public class DeviceFlowAuthenticator : IAuthenticator
    {
        private String ClientID = "37a11ee844d5eea41346";

        public event IAuthenticator.PopupRequestedEventHandler UserCodeReceived = delegate { };
        public event IAuthenticator.AuthenticatedEventHandler AuthenticationSuccess = delegate { };

        public async Task RunAuthenticate(GitHubClient Client)
        {
            if (Client.Credentials.AuthenticationType != AuthenticationType.Anonymous) {
                Console.WriteLine("Already logged in");
                return;
            }

            var token = await GetAuthToken(Client);

            if (token is OauthToken valid_token)
            {
                var credentials = new Credentials(valid_token.AccessToken);
                Client.Credentials = credentials;
                AuthenticationSuccess?.Invoke();
            }
        }

        private async Task<OauthToken?> GetAuthToken(GitHubClient Client)
        {
            var request = new OauthDeviceFlowRequest(ClientID);

            var deviceFlowResponse = await Client.Oauth.InitiateDeviceFlow(request);
            UserCodeReceived?.Invoke(deviceFlowResponse.UserCode);
            OpenInBrowser(deviceFlowResponse.VerificationUri);

            var token = await Client.Oauth.CreateAccessTokenForDeviceFlow(ClientID, deviceFlowResponse);

            return token;
        }

        private void OpenInBrowser(String LoginUrl)
        {
            try
            {
                var ps = new ProcessStartInfo(LoginUrl)
                {
                    UseShellExecute = true,
                    Verb = "open"
                };
                Process.Start(ps);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
