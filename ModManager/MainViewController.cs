using Imya.UI.Utils;
using Imya.UI.Views;
using Imya.Utils;
using System.ComponentModel;
using System.Windows.Controls;

namespace Imya.UI
{
    public enum View { 
        MOD_ACTIVATION,
        SETTINGS,
        TWEAKER,
        GAME_SETUP,
        MODINFO_CREATOR,
        MOD_INSTALLATION,
        GITHUB_BROWSER,
    }

    public class MainViewController : INotifyPropertyChanged
    {
        private static readonly ModActivationView _modActivation = new();
        private static readonly SettingsView _settings = new();
        private static readonly ModTweakerView _tweaker = new();
        private static readonly GameSetupView _gameSetup = new();
        private static readonly ModinfoCreatorView _modinfoCreator = new();
        private static readonly InstallationView _modInstallation = new();
        private static readonly GithubBrowserView _githubBrowser = new();

        public static MainViewController Instance { get; } = new();

        public delegate void ViewChangedEventHandler(View view);
        public event ViewChangedEventHandler ViewChanged = delegate { };

        public static readonly View DefaultView = View.MOD_ACTIVATION;
        public static readonly UserControl DefaultControl = _modInstallation;

        private MainViewController()
        {
            _currentView = GetViewControl(DefaultView);
        }

        private View _lastView = DefaultView;
        private UserControl _currentView;
        public UserControl CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }

        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public void SetView(View view)
        {
            View currentView = GetView();
            if (view == currentView)
                return;
            _lastView = currentView;

            if (currentView == View.TWEAKER && view != View.TWEAKER)
            {
                if (TweakManager.Instance.HasUnsavedChanges)
                {
                    var dialog = PopupCreator.CreateSaveTweakPopup();
                    if (dialog.ShowDialog() is true)
                        TweakManager.Instance.Save();
                }
            }
            CurrentView = GetViewControl(view);

            if (CurrentView is IViewPage page)
                page.OnLoad();
            ViewChanged(view);
        }

        private static UserControl GetViewControl(View view)
        {
            return view switch
            {
                View.MOD_ACTIVATION => _modActivation,
                View.SETTINGS => _settings,
                View.TWEAKER => _tweaker,
                View.GAME_SETUP => _gameSetup,
                View.MODINFO_CREATOR => _modinfoCreator,
                View.GITHUB_BROWSER => _githubBrowser,
                View.MOD_INSTALLATION => _modInstallation,
                _ => DefaultControl,
            };
        }

        public void GoToLastView()
        {
            SetView(_lastView);
            _lastView = DefaultView;
        }

        public View GetView()
        {
            return CurrentView switch
            {
                ModActivationView => View.MOD_ACTIVATION,
                SettingsView => View.SETTINGS,
                ModTweakerView => View.TWEAKER,
                GameSetupView => View.GAME_SETUP,
                ModinfoCreatorView => View.MODINFO_CREATOR,
                GithubBrowserView => View.GITHUB_BROWSER,
                InstallationView => View.MOD_INSTALLATION,
                _ => DefaultView,
            };
        }
    }
}
