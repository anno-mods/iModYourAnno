using System.Windows;
using Imya.Utils;
using Imya.Models;
using Imya.UI.Properties;
using Imya.Enums;

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

            //Set App Language
            TextManager.Instance.ChangeLanguage(ApplicationLanguage.English);
        }
    }
}
