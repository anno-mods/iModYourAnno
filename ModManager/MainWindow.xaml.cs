using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Anno.Utils;
using Imya.Services;
using Imya.Services.Interfaces;
using Imya.UI.Components;
using Imya.UI.Models;
using Imya.UI.Properties;
using Imya.UI.Utils;
using Imya.Utils;
using Octokit;

namespace Imya.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Properties.Settings Settings { get; } = Properties.Settings.Default;

        public IMainViewController MainViewController { get; init; }

        private IAuthenticator _authenticator;
        private IGameSetupService _gameSetupService;
        private PopupCreator _popupCreator;

        public Dashboard Dashboard { get; set; }
        public RichTextBox ConsoleLogTextBox { get; set; }

        public MainWindow(
            IAuthenticator authenticator,
            IMainViewController mainViewController,
            IGameSetupService gameSetupService,
            Dashboard dashboard,
            RichTextBox TextBox,
            PopupCreator popupCreator,
            SelfUpdater selfUpdater)
        {
            _authenticator = authenticator;
            _gameSetupService = gameSetupService;
            _popupCreator = popupCreator;
            Dashboard = dashboard;
            ConsoleLogTextBox = TextBox; 

            MainViewController = mainViewController;

            InitializeComponent();
            SetUpEmbeddedConsole();

            Console.WriteLine("Modders gonna take over the world!!!");
            DataContext = this;
            MinHeight = Settings.Default.MinWindowHeight;
            MinWidth = Settings.Default.MinWindowWidth;

            

            var productVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion ?? "?";
            Title = $"iModYourAnno - Anno 1800 Mod Manager {productVersion}";

            MainViewController.SetView(View.MOD_ACTIVATION);
#if DEBUG
            Properties.Settings.Default.DevMode = true;
#endif
            if (authenticator.HasStoredLoginInfo())
            {
                Task.Run(async () => await authenticator.StartAuthentication());
            }

            if (_gameSetupService.NeedsModloaderRemoval())
            {
                var result = _popupCreator.CreateModloaderPopup().ShowDialog();
                if (result is true)
                    _gameSetupService.RemoveModloader();
            }

            // initialize self-updater
            if (!Settings.DevMode)
                selfUpdater.CheckForUpdate("anno-mods", "iModYourAnno");
        }

        public void SetUpEmbeddedConsole()
        {
        }
    }
}
