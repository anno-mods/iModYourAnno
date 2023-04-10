using Imya.Services;
using Imya.Services.Interfaces;
using Imya.UI.Utils;
using Imya.UI.Views;
using System.ComponentModel;
using System.Windows.Controls;

namespace Imya.UI
{
    public enum View { 
        MOD_ACTIVATION,
        SETTINGS,
        TWEAKER,
        MODINFO_CREATOR,
        MOD_INSTALLATION,
        GITHUB_BROWSER
    }

    public class MainViewController : INotifyPropertyChanged, IMainViewController
    {
        private readonly ModActivationView _modActivation;
        private readonly SettingsView _settings;
        private readonly ModTweakerView _tweaker;
        private readonly ModinfoCreatorView _modinfoCreator;
        private readonly InstallationView _modInstallation;
        private readonly GithubBrowserView _githubBrowser;
        private readonly PopupCreator _popupCreator;

        public event IMainViewController.ViewChangedEventHandler ViewChanged = delegate { };

        public readonly View DefaultView = View.MOD_ACTIVATION;
        public readonly UserControl DefaultControl;

        private ITweakService _tweakService;
        private View _lastView;

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

        private MainViewController(
            ITweakService tweakService,
            ModActivationView modActivation,
            SettingsView settings,
            ModTweakerView tweaker,
            ModinfoCreatorView modinfoCreator,
            InstallationView modInstallation,
            GithubBrowserView githubBrowser,
            PopupCreator popupCreator)
        {
            _modActivation = modActivation;
            _settings = settings;
            _tweaker = tweaker;
            _modinfoCreator = modinfoCreator;
            _modInstallation = modInstallation;
            _githubBrowser = githubBrowser;
            _tweakService = tweakService;
            _popupCreator = popupCreator;

            DefaultControl = _modActivation;
            _currentView = GetViewControl(DefaultView);
            _lastView = DefaultView;

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
                if (_tweakService.HasUnsavedChanges)
                {
                    var dialog = _popupCreator.CreateSaveTweakPopup();
                    if (dialog.ShowDialog() is true)
                        _tweakService.Save();
                }
            }
            CurrentView = GetViewControl(view);

            if (CurrentView is IViewPage page)
                page.OnLoad();
            ViewChanged(view);
        }

        private UserControl GetViewControl(View view)
        {
            return view switch
            {
                View.MOD_ACTIVATION => _modActivation,
                View.SETTINGS => _settings,
                View.TWEAKER => _tweaker,
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
                ModinfoCreatorView => View.MODINFO_CREATOR,
                GithubBrowserView => View.GITHUB_BROWSER,
                InstallationView => View.MOD_INSTALLATION,
                _ => DefaultView,
            };
        }
    }
}
