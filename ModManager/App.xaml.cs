using System;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ModManager_Classes.src.Handlers;
using ModManager_Classes.src.Enums;
using ModManager.Properties;
namespace ModManager
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
            TextManager.Instance.ChangeLanguage(ApplicationLanguage.English);
        }
    }
}
