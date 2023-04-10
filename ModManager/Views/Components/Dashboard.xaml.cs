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
using Imya.UI.Models;
using Imya.Models.GameLauncher;
using Imya.Services;
using Imya.Services.Interfaces;
using Imya.Texts;
using Imya.Models.Mods;

namespace Imya.UI.Components
{
    /// <summary>
    /// Interaktionslogik für Dashboard.xaml
    /// </summary>
    public partial class Dashboard : UserControl, INotifyPropertyChanged
    {
        public ITextManager TextManager { get; }
        public Properties.Settings Settings { get; } = Properties.Settings.Default;

        public IGameSetupService GameSetupManager { get; init; }
        public IMainViewController MainViewController { get; }
        public IAppSettings AppSettings { get; init; }
        public IAuthenticationController AuthenticationController { get; init; }
        
        public IAuthenticator Authenticator { get; }
        private ITweakService _tweakService;
        private IGameLauncherFactory _launcherFactory;
        private ModCollection _globalMods;

        private PopupCreator _popupCreator;

        public bool CanStartGame { 
            get => _canStartGame;
            private set {
                _canStartGame = value;
                OnPropertyChanged(nameof(CanStartGame));
            }
        }
        private bool _canStartGame;

        public Dashboard(
            IAuthenticator authenticator,
            ITweakService tweakService,
            IAppSettings appSettings,
            IMainViewController mainViewController,
            IGameLauncherFactory gameLauncherFactory,
            IGameSetupService gameSetupService,
            ITextManager textManager,
            PopupCreator popupCreator,
            IAuthenticationController authController,
            ModCollection globalMods)
        {
            Authenticator = authenticator;
            AppSettings = appSettings;
            AuthenticationController = authController;
            MainViewController = mainViewController;
            TextManager = textManager;
            GameSetupManager = gameSetupService;

            _launcherFactory = gameLauncherFactory;
            _popupCreator = popupCreator;
            _tweakService = tweakService; 
            _globalMods = globalMods;

            InitializeComponent();
            DataContext = this;

            MainViewController.ViewChanged += UpdateSelection;
            CanStartGame = CheckCanStartGame();
        }

        private bool CheckCanStartGame()
        { 
            return GameSetupManager.IsValidSetup && !GameSetupManager.IsGameRunning;
        }

        public void SettingsClick(object sender, RoutedEventArgs e) => MainViewController.SetView(View.SETTINGS);

        public void ModManagementClick(object sender, RoutedEventArgs e) => MainViewController.SetView(View.MOD_ACTIVATION);

        public void BrowserClick(object sender, RoutedEventArgs e) => MainViewController.SetView(View.GITHUB_BROWSER);

        public void GameSetupClick(object sender, RoutedEventArgs e) => MainViewController.SetView(View.GITHUB_BROWSER);

        public void ModTweakerClick(object sender, RoutedEventArgs e) => MainViewController.SetView(View.TWEAKER);

        public void MetadataClick(object sender, RoutedEventArgs e) => MainViewController.SetView(View.MODINFO_CREATOR);

        public void ModInstallationClick(object sender, RoutedEventArgs e) => MainViewController.SetView(View.MOD_INSTALLATION);

        public void StartGameClick(object sender, RoutedEventArgs e)
        {
            var withUnresolved = _globalMods.WithAttribute(AttributeType.UnresolvedDependencyIssue);
            var withIncompatibleIssue = _globalMods.WithAttribute(AttributeType.ModCompabilityIssue);

            if (withUnresolved?.Count() > 0 || withIncompatibleIssue?.Count() > 0)
            {
                GenericOkayPopup popup = _popupCreator.CreateInvalidSetupPopup();
                if (popup.ShowDialog() is false) return;
            }

            if (_tweakService.HasUnsavedChanges)
            {
                var dialog = _popupCreator.CreateSaveTweakPopup();
                if (dialog.ShowDialog() is false) return;
            }

            StartGame(); 
        }

        private void StartGame()
        {
            var launcher = _launcherFactory.GetLauncher();
            launcher.GameExited += (a, b) =>
            {
                GameSetupManager.IsGameRunning = false;
                launcher.Dispose();
            };
            launcher.GameStarted += () => GameSetupManager.IsGameRunning = true;
            launcher.StartGame();
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
                case View.MOD_INSTALLATION:
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
            var dialogresult = _popupCreator.CreateLogoutPopup().ShowDialog();
            if (dialogresult is false) return;

            AuthenticationController.Logout();
        }
    }
}
