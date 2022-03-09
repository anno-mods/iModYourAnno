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

namespace Imya.UI.Components
{
    /// <summary>
    /// Interaktionslogik für Dashboard.xaml
    /// </summary>
    public partial class Dashboard : UserControl, INotifyPropertyChanged
    {
        public TextManager TextManager { get; } = TextManager.Instance;
        public SettingsManager SettingsManager { get; } = SettingsManager.Instance;
        public GameSetupManager GameSetupManager { get; } = GameSetupManager.Instance;

        static ModActivationView ModActivationView = new ModActivationView();
        static SettingsView SettingsView = new SettingsView();
        static ModTweakerView TweakerView = new ModTweakerView();
        static GameSetupView GameSetupView = new GameSetupView();
        static ModinfoCreatorView ModinfoCreatorView = new ModinfoCreatorView();

        private static UserControl DummyControl = new DummyControl();


        private UserControl _currentView;
        public UserControl CurrentView {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }

        public Dashboard()
        {
            InitializeComponent();
            DataContext = this;

            CurrentView = ModActivationView;
        }

        public void SettingsClick(object sender, RoutedEventArgs e)
        {
            CurrentView = SettingsView;
        }

        public void ModManagementClick(object sender, RoutedEventArgs e)
        {
            CurrentView = ModActivationView;
        }

        public void GameSetupClick(object sender, RoutedEventArgs e)
        {
            CurrentView = GameSetupView;
        }

        public void ModTweakerClick(object sender, RoutedEventArgs e)
        {
            CurrentView = TweakerView;
        }

        public void MetadataClick(object sender, RoutedEventArgs e)
        {
            CurrentView = ModinfoCreatorView;
        }

        public void ModInstallationClick(object sender, RoutedEventArgs e)
        {
            CurrentView = DummyControl;
        }

        public void StartGameClick(object sender, RoutedEventArgs e)
        {
            GameSetupManager.Instance.StartGame();
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
    }


}
