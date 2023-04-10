using Imya.Services;
using Imya.Services.Interfaces;
using Imya.UI.Utils;
using Imya.UI.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
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

        IServiceProvider _serviceProvider; 

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

        public MainViewController(
            ITweakService tweakService,
            IServiceProvider serviceProvider,
            PopupCreator popupCreator)
        {
            _serviceProvider = serviceProvider;
            _tweakService = tweakService;
            _popupCreator = popupCreator;

            DefaultControl = _serviceProvider.GetRequiredService<ModActivationView>();
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
                View.MOD_ACTIVATION => _serviceProvider.GetRequiredService<ModActivationView>(),
                View.SETTINGS => _serviceProvider.GetRequiredService<SettingsView>(),
                View.TWEAKER => _serviceProvider.GetRequiredService<ModTweakerView>(),
                View.MODINFO_CREATOR => _serviceProvider.GetRequiredService<ModinfoCreatorView>(),
                View.GITHUB_BROWSER => _serviceProvider.GetRequiredService<GithubBrowserView>(),
                View.MOD_INSTALLATION => _serviceProvider.GetRequiredService<InstallationView>(),
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
