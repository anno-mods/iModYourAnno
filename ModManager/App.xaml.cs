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

            //init the settings manager for reasons
            SettingsManager settingsManager = new SettingsManager();

            //Set App Language
            TextManager.Instance.ChangeLanguage(ApplicationLanguage.English);

            ModinfoCreationManager modInfoCreationManager = new ModinfoCreationManager();
        }
    }
}
