using Imya.Models.NotifyPropertyChanged;
using Imya.Utils;
using Octokit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CredentialManagement;

namespace Imya.UI.Utils
{
    public class DeviceFlowAuthenticator : IAuthenticator
    {
        private String ClientID = "37a11ee844d5eea41346";
        private const String tokenCredentialId = "IMYA_OAUTH_TOKEN";

        public event IAuthenticator.PopupRequestedEventHandler UserCodeReceived = delegate { };
        public event IAuthenticator.AuthenticatedEventHandler AuthenticationSuccess = delegate { };

        private IGitHubClient _client;

        public DeviceFlowAuthenticator(IGitHubClient client)
        {
            _client = client;
        }

        public async Task StartAuthentication()
        {
            if (_client.Connection.Credentials.AuthenticationType != AuthenticationType.Anonymous) {
                Console.WriteLine("Already logged in");
                return;
            }

            String? access_token = null;
            //use saved token. if it doesn't exist, create a token request and save the result.
            if (!TryGetSavedOauthToken(out access_token))
            {
                var token = await GetAuthToken();
                if (!(token is OauthToken valid_token))
                    return;

                access_token = valid_token.AccessToken;
                SaveOauthToken(access_token);
            }
            var credentials = new Credentials(access_token);
            _client.Connection.Credentials = credentials;
            AuthenticationSuccess?.Invoke();
        }

        private async Task<OauthToken?> GetAuthToken()
        {
            var request = new OauthDeviceFlowRequest(ClientID);

            var deviceFlowResponse = await _client.Oauth.InitiateDeviceFlow(request);
            UserCodeReceived?.Invoke(deviceFlowResponse.UserCode);
            OpenInBrowser(deviceFlowResponse.VerificationUri);

            var token = await _client.Oauth.CreateAccessTokenForDeviceFlow(ClientID, deviceFlowResponse);
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

        public bool HasStoredLoginInfo()
        {
            using (var cred = new Credential() { Target = tokenCredentialId })
            {
                return cred.Exists();
            }
        }

        private void SaveOauthToken(String token)
        {
            using (var cred = new Credential() { Target = tokenCredentialId })
            {
                cred.Password = token;
                cred.Type = CredentialType.Generic;
                cred.PersistanceType = PersistanceType.LocalComputer;
                cred.Save();
            }
        }

        private bool TryGetSavedOauthToken(out String token)
        {
            using (var cred = new Credential() { Target = tokenCredentialId })
            {
                token = "";
                if (!cred.Exists()) return false;
                cred.Load();
                token = cred.Password;
                return true;
            }
        }

        private void CleanSavedOauthToken()
        {
            using (var cred = new Credential() { Target = tokenCredentialId })
            {
                cred.Delete();
            }
        }

        public void RemoveAuthentication()
        {
            CleanSavedOauthToken();
        }
    }
}
