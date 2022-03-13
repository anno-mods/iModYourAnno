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
            //Load Texts asap.
            TextManager TextManager = new TextManager();
            TextManager.LoadLanguageFile(Settings.Default.LanguageFilePath);

            GameSetupManager GameSetupManager = new GameSetupManager(); 
            GameSetupManager.SetGamePath(Settings.Default.GameRootPath, true);
            GameSetupManager.SetModDirectoryName(Settings.Default.ModDirectoryName);

            InstallationManager InstallationManager = new InstallationManager(Settings.Default.DownloadDir);

            //Setup Managers
            ModDirectoryManager ModDirectoryManager = new ModDirectoryManager();

            ModinfoCreationManager modInfoCreationManager = new ModinfoCreationManager();
        }
    }
}
