using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Anno.Utils;
using Imya.UI.Properties;
using Imya.UI.Utils;
using Imya.Utils;

namespace Imya.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Properties.Settings Settings { get; } = Properties.Settings.Default;

        public MainViewController MainViewController { get; } = MainViewController.Instance;

        public MainWindow()
        {
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

            if (GithubClientProvider.Authenticator.HasStoredLoginInfo())
            {
                Task.Run(async () => await GithubClientProvider.Authenticator.StartAuthentication());
            }

            if (GameSetupManager.Instance.NeedsModloaderRemoval())
            {
                var result = PopupCreator.CreateModloaderPopup().ShowDialog();
                if (result is true)
                    GameSetupManager.Instance.RemoveModloader();
            }

            // initialize self-updater
            SelfUpdater.CheckForUpdate(GithubClientProvider.Client, "anno-mods", "iModYourAnno");
        }

        public void SetUpEmbeddedConsole()
        {
            Console.SetOut(new EmbeddedConsole(ConsoleLogTextBox.ConsoleOut, this));
        }
    }
}
