using Imya.Models;
using Imya.Models.NotifyPropertyChanged;
using Imya.UI.Popup;
using Imya.UI.Utils;
using Imya.Utils;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.UI.Models
{
    public class AuthenticationController : PropertyChangedNotifier, IAuthenticationController
    {
        private AuthCodePopup? AuthCodePopup;

        public bool IsAuthenticated
        {
            get => _isAuthenticated;
            set => SetProperty(ref _isAuthenticated, value);
        }
        private bool _isAuthenticated = false;

        public string? AuthenticatedUser
        {
            get => _authenticatedUser;
            set => SetProperty(ref _authenticatedUser, value);
        }
        private string? _authenticatedUser;

        public Uri? AvatarUri
        {
            get => _uri;
            set => SetProperty(ref _uri, value);
        }
        private Uri? _uri;

        private IAuthenticator _authenticator;
        private IGitHubClient _client;
        private readonly PopupCreator _popupCreator;

        public AuthenticationController(
            IGitHubClient client, 
            IAuthenticator authenticator,
            PopupCreator popupCreator)
        {
            _authenticator = authenticator;
            _client = client;
            _popupCreator = popupCreator;

            _authenticator.UserCodeReceived += OnAuthCodeReceived;
            _authenticator.AuthenticationSuccess += OnAuthSuccess;
        }

        public void Authenticate()
        {
            Task.Run(async () => await _authenticator.StartAuthentication());
        }

        public void Logout()
        {
            _client.Connection.Credentials = Octokit.Credentials.Anonymous;
            IsAuthenticated = false;
            AvatarUri = null;
            AuthenticatedUser = null;
            _authenticator.RemoveAuthentication();
        }

        private void OnAuthCodeReceived(string AuthCode)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                if (AuthCodePopup is AuthCodePopup)
                    AuthCodePopup.Close();
                AuthCodePopup = _popupCreator.CreateAuthCodePopup(AuthCode);
                AuthCodePopup.Show();
            });
        }

        private void OnAuthSuccess()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                IsAuthenticated = true;
                AuthCodePopup?.Close();
                Task.Run(async () => await UpdateUserLogin());
            });
        }

        private async Task UpdateUserLogin()
        {
            var user = await _client.User.Current();
            AuthenticatedUser = user.Login;
            Console.WriteLine($"Authenticated as {AuthenticatedUser}");
            AvatarUri = new Uri(user.AvatarUrl);
        }
    }
}
