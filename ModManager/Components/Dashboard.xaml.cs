using Imya.Models;
using Imya.Utils;
using System.Windows;
using System.Windows.Controls;
using Imya.Enums;
using System.ComponentModel;
using System.Windows.Data;
using System;
using System.Globalization;

namespace Imya.UI.Components
{
    /// <summary>
    /// Interaktionslogik für Dashboard.xaml
    /// </summary>
    public partial class Dashboard : UserControl, INotifyPropertyChanged
    {
        public TextManager TextManager { get; } = TextManager.Instance;

        public IText ModInstallationText { get; } = TextManager.Instance.GetText("DASHBOARD_MOD_INSTALLATION");
        public IText GameSetupText { get; } = TextManager.Instance.GetText("DASHBOARD_GAME_SETUP");
        public IText ModActivationText { get; } = TextManager.Instance.GetText("DASHBOARD_MOD_ACTIVATION");
        public IText PlayText { get; } = TextManager.Instance.GetText("DASHBOARD_PLAY");
        public IText DashboardText { get; } = TextManager.Instance.GetText("DASHBOARD_DASHBOARD");

        private int _currentViewIndex;
        public int CurrentViewIndex {
            get => _currentViewIndex;
            set
            { 
                _currentViewIndex = value;
                OnPropertyChanged(nameof(CurrentViewIndex));
            }
        }

        public Dashboard()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void OnClick(object sender, RoutedEventArgs e)
        {
            if(TextManager.Instance.ApplicationLanguage == ApplicationLanguage.English)
                TextManager.Instance.ChangeLanguage(ApplicationLanguage.German);
            else
                TextManager.Instance.ChangeLanguage(ApplicationLanguage.English);
        }

        public void SettingsClick(object sender, RoutedEventArgs e)
        {
            CurrentViewIndex = 1;
        }

        public void ModManagementClick(object sender, RoutedEventArgs e)
        {
            CurrentViewIndex = 0;
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
