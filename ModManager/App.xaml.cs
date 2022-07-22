using System.Windows;
using Imya.Utils;
using Imya.Models;
using Imya.UI.Properties;
using Imya.Enums;
using Imya.UI.Utils;
using System.Threading.Tasks;
using Imya.Models.Options;
using Imya.Models.Attributes;
using Imya.UI.Models;

namespace Imya.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
       public App()
       {
            // load localized text first
            var text = TextManager.Instance;
            text.LoadLanguageFile(Settings.Default.LanguageFilePath);

            var gameSetup = GameSetupManager.Instance;
            //gameSetup.SetDownloadDirectory(Settings.Default.DownloadDir);
            gameSetup.SetGamePath(Settings.Default.GameRootPath, true);
            gameSetup.SetModDirectoryName(Settings.Default.ModDirectoryName);

            AttributeCollectionFactory.AttributeCollectionType = typeof(ObservableAttributeCollection);

            // init global mods
            ModCollection.Global = new ModCollection(gameSetup.GetModDirectory(), new ModCollectionOptions()
            {
                Normalize = true,
                LoadImages = true
            });
            ModCollectionHooks.Initialize();
            Task.Run(() => ModCollection.Global.LoadModsAsync());
        }
    }
}
