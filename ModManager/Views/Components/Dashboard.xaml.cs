using Imya.Models;
using Imya.Utils;
using System.Windows;
using System.Windows.Controls;
using Imya.Enums;
using System.ComponentModel;
using System.Windows.Data;
using System;
using System.Globalization;
using Imya.UI.Views;
using Imya.UI.Utils;
using System.Runtime.CompilerServices;
using Imya.Models.Attributes;
using System.Linq;
using Imya.UI.Popup;
using Imya.UI.Views.Models;

namespace Imya.UI.Components
{
    /// <summary>
    /// Interaktionslogik für Dashboard.xaml
    /// </summary>
    public partial class Dashboard : UserControl, INotifyPropertyChanged
    {
        public TextManager TextManager { get; } = TextManager.Instance;
        public Properties.Settings Settings { get; } = Properties.Settings.Default;
        public GameSetupManager GameSetupManager { get; } = GameSetupManager.Instance;
        public MainViewController MainViewController { get; } = MainViewController.Instance;

        public AuthenticationController AuthenticationController { get; } = new AuthenticationController();
        public IAuthenticator Authenticator { get; } = GithubClientProvider.Authenticator;

        public bool CanStartGame { 
            get => _canStartGame;
            private set {
                _canStartGame = value;
                OnPropertyChanged(nameof(CanStartGame));
            }
        }
        private bool _canStartGame;

        public Dashboard()
        {
            InitializeComponent();
            DataContext = this;

            MainViewController.Instance.ViewChanged += UpdateSelection;

            CanStartGame = CheckCanStartGame();

            GameSetupManager.GameStarted += () => CanStartGame = CheckCanStartGame();
            GameSetupManager.GameClosed += (x, y) => CanStartGame = CheckCanStartGame();
        }

        private bool CheckCanStartGame()
        { 
            return GameSetupManager.IsValidSetup && !GameSetupManager.IsGameRunning;
        }

        public void SettingsClick(object sender, RoutedEventArgs e) => MainViewController.SetView(View.SETTINGS);

        public void ModManagementClick(object sender, RoutedEventArgs e) => MainViewController.SetView(View.MOD_ACTIVATION);

        public void GameSetupClick(object sender, RoutedEventArgs e) => MainViewController.SetView(View.GAME_SETUP);

        public void ModTweakerClick(object sender, RoutedEventArgs e) => MainViewController.SetView(View.TWEAKER);

        public void MetadataClick(object sender, RoutedEventArgs e) => MainViewController.SetView(View.MODINFO_CREATOR);

        public void ModInstallationClick(object sender, RoutedEventArgs e) => MainViewController.SetView(View.DUMMY);

        public void StartGameClick(object sender, RoutedEventArgs e)
        {
            var withUnresolved = ModCollection.Global?.WithAttribute(AttributeType.UnresolvedDependencyIssue);
            var withIncompatibleIssue = ModCollection.Global?.WithAttribute(AttributeType.ModCompabilityIssue);

            if (withUnresolved?.Count() > 0 || withIncompatibleIssue?.Count() > 0)
            {
                GenericOkayPopup popup = new GenericOkayPopup()
                {
                    MESSAGE = TextManager.GetText("ATTRIBUTE_GAMESTART_WARNING")
                };
                if (popup.ShowDialog() is false) return;
            }

            GameSetupManager.Instance.StartGame();
        }

        private void UpdateSelection(View view)
        {
            var radioButton = GetButton(view);
            if(radioButton is not null) radioButton.IsChecked = true;
        }

        private RadioButton? GetButton(View view)
        {
            switch (view)
            {
                case View.MOD_ACTIVATION:
                    return ModManagementButton;
                case View.GAME_SETUP:
                    return ModInstallationButton;
                case View.TWEAKER:
                    return ModTweakerButton;
                case View.MODINFO_CREATOR:
                    return ModinfoCreatorButton;
                default: return null;
            }
        }


        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler? PropertyChanged = delegate { };
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        private void SetProperty<T>(ref T property, T value, [CallerMemberName] string propertyName = "")
        {
            property = value;
            OnPropertyChanged(propertyName);
        }
        #endregion

        private void LoginButtonClick(object sender, RoutedEventArgs e)
        {
            AuthenticationController.Authenticate();
        }

        private void LogoutButtonClick(object sender, RoutedEventArgs e)
        {
            AuthenticationController.Logout();
        }
    }
}
