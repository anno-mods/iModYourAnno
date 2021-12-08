using Imya.Models;
using Imya.Utils;
using System.Windows;
using System.Windows.Controls;
using Imya.Enums;

namespace Imya_UI.Components
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

        public Dashboard()
        {
            InitializeComponent();
            DataContext = this;
        }

        //This is just a placeholder that changes languages on Settings Button Click.
        public void OnClick(object sender, RoutedEventArgs e)
        {
            if(TextManager.Instance.ApplicationLanguage == ApplicationLanguage.English)
                TextManager.Instance.ChangeLanguage(ApplicationLanguage.German);
            else
                TextManager.Instance.ChangeLanguage(ApplicationLanguage.English);
        }
    }
}
