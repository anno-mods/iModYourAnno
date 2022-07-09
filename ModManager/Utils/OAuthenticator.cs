using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Imya.Utils;
using Newtonsoft.Json;
using Octokit;

namespace Imya.UI.Utils
{
    internal class OAuthenticator : IAuthenticator
    {
        private GitHubClient _client;
        private IAuthenticator.Secrets secrets;

        public async Task RunAuthenticate(GitHubClient client)
        {
            if (secrets.Equals(default(IAuthenticator.Secrets)))
            {
                Console.WriteLine("Load secrets first!!!");
                return;
            }

            _client = client;
            await RunAuthorize();
        }

        public IAuthenticator.Secrets LoadSecrets(String Filename)
        {
            if (!File.Exists(Filename))
            {
                Console.WriteLine("secret file unavailable");
            }
            String json = File.ReadAllText(Filename);
            secrets = JsonConvert.DeserializeObject<IAuthenticator.Secrets>(json);
            return secrets;
        }

        private async Task RunAuthorize()
        {
            var state = randomDataBase64url(32);

            //String redirectUrl = String.Format("http://{0}:{1}/", IPAddress.Loopback, GetRandomUnusedPort());
            String redirectUrl = "http://localhost:8888/";


            String LoginUrl = GetAuthLoginUrl(state, new Uri(redirectUrl));
            OpenInBrowser(LoginUrl);

            var http = new HttpListener();
            http.Prefixes.Add(redirectUrl);
            http.Start();

            HttpListenerContext? context = null;
            try
            {
                context = await http.GetContextAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            // Sends an HTTP response to the browser.
            var response = context.Response;
            string responseString = string.Format("<html><head></head><body>Please return to the app</body></html>");
            var buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            var responseOutput = response.OutputStream;
            Task responseTask = responseOutput.WriteAsync(buffer, 0, buffer.Length).ContinueWith((task) =>
            {
                responseOutput.Close();
                http.Stop();
                Console.WriteLine("HTTP server stopped.");
            });

            // Checks for errors.
            if (context.Request.QueryString.Get("error") != null)
            {
                Console.WriteLine(String.Format("OAuth authorization error: {0}.", context.Request.QueryString.Get("error")));
                return;
            }
            if (context.Request.QueryString.Get("code") == null
                || context.Request.QueryString.Get("state") == null)
            {
                Console.WriteLine("Malformed authorization response. " + context.Request.QueryString);
                return;
            }

            // extracts the code
            var code = context.Request.QueryString.Get("code");
            var incoming_state = context.Request.QueryString.Get("state");

            if (incoming_state != state)
            {
                Console.WriteLine(String.Format("Received request with invalid state ({0})", incoming_state));
                return;
            }

            await Authorize(code!);
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

        private String GetAuthLoginUrl(string state, Uri RedirectUri)
        {
            var request = new OauthLoginRequest(secrets.ClientId)
            {
                Scopes = { "user", "notifications" },
                State = state,
                RedirectUri = RedirectUri
            };

            var oauthLoginUrl = _client.Oauth.GetGitHubLoginUrl(request);
            return oauthLoginUrl.ToString();
        }


        // ref http://stackoverflow.com/a/3978040
        public int GetRandomUnusedPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }


        public async Task Authorize(string code)
        {
            if (String.IsNullOrEmpty(code)) return;

            var request = new OauthTokenRequest(secrets.ClientId, secrets.ClientSecret, code);
            var token = await _client.Oauth.CreateAccessToken(request);

            var credentials = new Credentials(token.AccessToken);
            _client.Credentials = credentials;

            var user = (await _client.User.Current());
            Console.WriteLine("Authenticated as: " + user.Login);

            //clean the secrets
            secrets = new();

            return;
        }

        public static string randomDataBase64url(uint length)
        {
            byte[] bytes = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            return base64urlencodeNoPadding(bytes);
        }

        public static string base64urlencodeNoPadding(byte[] buffer)
        {
            string base64 = Convert.ToBase64String(buffer);

            // Converts base64 to base64url.
            base64 = base64.Replace("+", "-");
            base64 = base64.Replace("/", "_");
            // Strips padding.
            base64 = base64.Replace("=", "");

            return base64;
        }
    }
}
