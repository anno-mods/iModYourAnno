using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ModManager_Classes.src.Enums;
using ModManager_Classes.src.Handlers;
using ModManager_Classes.src.Models;

namespace ModManager_Views
{
    /// <summary>
    /// Interaktionslogik für Dashboard.xaml
    /// </summary>
    public partial class Dashboard : UserControl
    {
        public LocalizedText SettingsText { get; set; }

        public Dashboard()
        {
            InitializeComponent();
            DataContext = this;

            //set texts
            SettingsText = TextManager.Instance.GetText("SETTINGS_TEXT");
        }

        public void OnClick(object sender, RoutedEventArgs e)
        {
            if(LanguageManager.Instance.ApplicationLanguage == ApplicationLanguage.English)
                LanguageManager.Instance.ChangeLanguage(ApplicationLanguage.German);
            else
                LanguageManager.Instance.ChangeLanguage(ApplicationLanguage.English);
        }
    }
}
