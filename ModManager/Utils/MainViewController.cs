using Imya.UI.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Imya.UI.Utils
{
    public enum View { 
        MOD_ACTIVATION,
        SETTINGS,
        TWEAKER,
        GAME_SETUP,
        MODINFO_CREATOR,
        DUMMY
    }


    public class MainViewController : INotifyPropertyChanged
    {
        private static ModActivationView ModActivationView = new ModActivationView();
        private static SettingsView SettingsView = new SettingsView();
        private static ModTweakerView TweakerView = new ModTweakerView();
        private static GameSetupView GameSetupView = new GameSetupView();
        private static ModinfoCreatorView ModinfoCreatorView = new ModinfoCreatorView();

        private static UserControl DummyControl = new DummyControl();

        public static MainViewController Instance { get; private set; }

        public MainViewController()
        {
            Instance ??= this;
        }

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

        public event PropertyChangedEventHandler? PropertyChanged = delegate { };

        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler is PropertyChangedEventHandler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        public void SetView(View view)
        {
            switch (view)
            {
                case View.MOD_ACTIVATION: CurrentView = ModActivationView; break;
                case View.SETTINGS: CurrentView = SettingsView; break;
                case View.TWEAKER: CurrentView = TweakerView; break;
                case View.GAME_SETUP: CurrentView = GameSetupView; break;
                case View.MODINFO_CREATOR: CurrentView = ModinfoCreatorView; break;

                default: CurrentView = DummyControl; break;
            }
        }

        public View GetView()
        {
            if (DummyControl is ModActivationView) return View.MOD_ACTIVATION;
            if (DummyControl is SettingsView) return View.SETTINGS;
            if (DummyControl is ModTweakerView) return View.TWEAKER;
            if (DummyControl is GameSetupView) return View.GAME_SETUP;
            if (DummyControl is ModinfoCreatorView) return View.MODINFO_CREATOR;

            return View.DUMMY;
        }
    }
}
