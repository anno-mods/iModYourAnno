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
            TextManager.LoadLanguageFile(Settings.Default.LANGUAGE_FILE_PATH);

            GameSetupManager GameSetupManager = new GameSetupManager(); 
            GameSetupManager.SetGamePath(Settings.Default.GAME_ROOT_PATH);
            GameSetupManager.RegisterModDirectoryName(Settings.Default.MOD_DIRECTORY_NAME);

            ModTweakingManager modTweakingManager = new ModTweakingManager();

            InstallationManager InstallationManager = new InstallationManager(Settings.Default.DOWNLOAD_DIR);

            //Setup Managers
            ModDirectoryManager ModDirectoryManager = new ModDirectoryManager();

            TextManager.Instance.ChangeLanguage(Settings.Default.Language);

            ModinfoCreationManager modInfoCreationManager = new ModinfoCreationManager();
        }
    }
}
