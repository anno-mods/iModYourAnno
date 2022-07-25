using Imya.UI.Popup;
using Imya.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.UI.Views.Models
{
    public class AuthenticationController
    {
        private AuthCodePopup? AuthCodePopup;

        public AuthenticationController()
        {
            GithubClientProvider.Authenticator.UserCodeReceived += ShowAuthCodePopup;
            GithubClientProvider.Authenticator.AuthenticationSuccess += CloseAuthCodePopup;
        }

        public void Authenticate()
        {
            Task.Run(async () => await GithubClientProvider.RunAuthenticate());
        }

        private void ShowAuthCodePopup(String AuthCode)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                if (AuthCodePopup is AuthCodePopup)
                    AuthCodePopup.Close();
                AuthCodePopup = new AuthCodePopup(AuthCode);
                AuthCodePopup.Show();
            });
        }

        private void CloseAuthCodePopup()
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                Console.WriteLine("Authenticated as: " + GithubClientProvider.Authenticator.AuthenticatedUser);
                AuthCodePopup?.Close();
            });
        }
    }
}
