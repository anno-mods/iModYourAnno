﻿using System.Windows;
using Imya.Utils;
using Imya.Models;
using Imya.UI.Properties;
using Imya.Enums;
using Imya.UI.Utils;
using System.Threading.Tasks;
using Imya.Models.Options;
using Imya.Models.Attributes;
using Imya.UI.Models;
using Imya.Validation;

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
            gameSetup.ModindexLocation = Settings.Default.ModindexLocation;

            GithubClientProvider.Authenticator = new DeviceFlowAuthenticator();

            // init global mods
            ModCollection.Global = new ModCollection(gameSetup.GetModDirectory(), normalize: true, loadImages: true);
            ModCollectionHooks.Initialize();
            Task.Run(() => ModCollection.Global.LoadModsAsync());
        }
    }
}
