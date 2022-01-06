using System.Windows;
using Imya.Utils;
using Imya.Models;
using Imya.UI.Properties;
using Imya.Enums;
using Imya.UI.Utils;

namespace Imya.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
       public App()
        {
            //Setup Managers
            TextManager TextManager = new TextManager(Settings.Default.LANGUAGE_FILE_PATH);
            ModDirectoryManager ModDirectoryManager = new ModDirectoryManager(Settings.Default.MOD_DIRECTORY_PATH);

            SettingsManager settingsManager = new SettingsManager(); 

            //Set App Language
            TextManager.Instance.ChangeLanguage(ApplicationLanguage.English);
        }
    }
}
