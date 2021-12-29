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
    public partial class Dashboard : UserControl
    {
        public LocalizedText SettingsText { get; } = TextManager.Instance.GetText("DASHBOARD_SETTINGS");
        public LocalizedText ModTweakingText { get; } = TextManager.Instance.GetText("DASHBOARD_MOD_TWEAKING");
        public LocalizedText ModInstallationText { get; } = TextManager.Instance.GetText("DASHBOARD_MOD_INSTALLATION");
        public LocalizedText GameSetupText { get; } = TextManager.Instance.GetText("DASHBOARD_GAME_SETUP");
        public LocalizedText ModActivationText { get; } = TextManager.Instance.GetText("DASHBOARD_MOD_ACTIVATION");
        public LocalizedText PlayText { get; } = TextManager.Instance.GetText("DASHBOARD_PLAY");
        public LocalizedText DashboardText { get; } = TextManager.Instance.GetText("DASHBOARD_DASHBOARD");

        public event MainViewChangedEventHandler MainViewChanged = delegate { };
        public delegate void MainViewChangedEventHandler(int indexToShow);

        public Dashboard()
        {
            InitializeComponent();
            DataContext = this;
            //ClearButtonRects();
        }

        //This is just a placeholder that changes languages on Settings Button Click.
        public void OnClick(object sender, RoutedEventArgs e)
        {
            if(TextManager.Instance.ApplicationLanguage == ApplicationLanguage.English)
                TextManager.Instance.ChangeLanguage(ApplicationLanguage.German);
            else
                TextManager.Instance.ChangeLanguage(ApplicationLanguage.English);
        }

        public void SettingsClick(object sender, RoutedEventArgs e)
        {
            //ClearButtonRects();
            MainViewChanged(1);
            //SettingsSelectionRect.Visibility = Visibility.Visible;
        }

        public void ModManagementClick(object sender, RoutedEventArgs e)
        {
            //ClearButtonRects();
            MainViewChanged(0);
            //ModManagementSelectionRect.Visibility = Visibility.Visible;
        }

        //look, I know this is completely retarded code.
        //I just gave up trying to create my own Control for this dashboard.

        /*private void ClearButtonRects()
        { 
            ModManagementSelectionRect.Visibility = Visibility.Hidden;
            GameSetupSelectionRect.Visibility=Visibility.Hidden;
            ModInstallSelectionRect.Visibility=Visibility.Hidden;
            ModTweakerSelectionRect.Visibility = Visibility.Hidden;
            SettingsSelectionRect.Visibility = Visibility.Hidden;
        }*/
    }

    
}
