using System.Windows;
using Imya.UI.Properties;
using Imya.UI.Utils;
using System.Threading.Tasks;
using Imya.Validation;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Imya.Services.Interfaces;
using Imya.Services;
using Imya.Models.Cache;
using Imya.GithubIntegration;
using System;
using Imya.Utils;
using Imya.Texts;
using Imya.UI.Views;
using Imya.GithubIntegration.StaticData;
using Imya.UI.Models;
using Imya.Models.Options;
using Imya.Models.GameLauncher;
using Imya.Models.Installation.Interfaces;
using Imya.Models.Installation;
using Imya.Models.Mods;
using System.Windows.Forms;
using Imya.Models.Attributes.Interfaces;
using Imya.Models.Attributes.Factories;
using Imya.Models.ModTweaker.DataModel.Storage;

namespace Imya.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private ModCollectionHooks _hooks;

        public static IHost AppHost { get; private set; }

        public App()
        {
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    //services
                    services.AddSingleton<ITextManager, TextManager>();
                    services.AddTransient<ICyclicDependencyAttributeFactory, CyclicDependencyAttributeFactory>();
                    services.AddTransient<IMissingModinfoAttributeFactory, MissingModinfoAttributeFactory>();
                    services.AddTransient<IModCompabilityAttributeFactory, ModCompabilityAttributeFactory>();
                    services.AddTransient<IModDependencyIssueAttributeFactory, ModDependencyIssueAttributeFactory>();
                    services.AddTransient<IModReplacedByAttributeFactory, ModReplacedByAttributeFactory>();
                    services.AddTransient<IModStatusAttributeFactory, ModStatusAttributeFactory>();
                    services.AddTransient<IRemovedFolderAttributeFactory, RemovedFolderAttributeFactory>();
                    services.AddTransient<ITweakedAttributeFactory, TweakedAttributeFactory>();
                    services.AddTransient<IContentInSubfolderAttributeFactory, ContentInSubfolderAttributeFactory>();

                    services.AddSingleton<IModFactory, ModFactory>();
                    services.AddSingleton<IModCollectionFactory, ModCollectionFactory>();
                    services.AddSingleton<IAppSettings, AppSettings>();
                    services.AddSingleton<IGameSetupService, GameSetupService>();
                    services.AddSingleton<IInstallationService, InstallationService>();
                    services.AddSingleton<ITweakService, TweakService>();

                    //github integration
                    services.AddSingleton<Octokit.IGitHubClient, Octokit.GitHubClient>();
                    services.AddTransient<IReadmeStrategy, StaticFilenameReadmeStrategy>();
                    services.AddTransient<IReleaseAssetStrategy, StaticNameReleaseAssetStrategy>();
                    services.AddTransient<IModImageStrategy, StaticFilepathImageStrategy>();
                    services.AddSingleton<IAuthenticator, DeviceFlowAuthenticator>();

                    //tweaks
                    services.AddSingleton<ITweakRepository, TweakRepository>();

                    //game launcher
                    services.AddSingleton<IGameLauncherFactory, GameLauncherFactory>();
                    services.AddSingleton<SteamGameLauncher>(serviceProvider => new SteamGameLauncher(
                                            serviceProvider.GetRequiredService<IGameSetupService>()));
                    services.AddSingleton<StandardGameLauncher>(serviceProvider => new StandardGameLauncher(
                                            serviceProvider.GetRequiredService<IGameSetupService>()));

                    //setup global mod collection
                    var lidlServiceProvider = services.BuildServiceProvider();
                    var factory = lidlServiceProvider.GetRequiredService<IModCollectionFactory>();
                    var gameSetup = lidlServiceProvider.GetRequiredService<IGameSetupService>();
                    var collection = factory.Get(gameSetup.GetModDirectory(), normalize: true, loadImages: true);
                    services.AddSingleton(collection);
                    services.AddSingleton<CyclicDependencyAttributeFactory>();

                    //hooks
                    services.AddSingleton<ModCollectionHooks>(serviceProvider => new ModCollectionHooks(
                                            serviceProvider.GetRequiredService<ITweakRepository>()));
                    services.AddSingleton<CyclicDependencyValidator>(serviceProvider => new CyclicDependencyValidator(
                                            serviceProvider.GetRequiredService<CyclicDependencyAttributeFactory>()));
                    services.AddSingleton<ModCompatibilityValidator>();
                    services.AddSingleton<ModContentValidator>();
                    services.AddSingleton<ModDependencyValidator>();
                    services.AddSingleton<ModReplacementValidator>();
                    services.AddSingleton<RemovedModValidator>();

                    //caching
                    services.AddScoped<ICache<GithubRepoInfo, String>, TimedCache<GithubRepoInfo, String>>();

                    //installation
                    services.AddScoped<IModInstallationOptions, ModInstallationOptions>();
                    services.AddScoped<IGithubDownloaderOptions, GithubDownloaderOptions>();
                    services.AddScoped<IModloaderInstallationOptions, ModloaderInstallationOptions>();
                    services.AddScoped<IGithubInstallationBuilderFactory, GithubInstallationBuilderFactory>();
                    services.AddScoped<IZipInstallationBuilderFactory, ZipInstallationBuilderFactory>();
                    services.AddSingleton<GithubInstallationBuilder>(serviceProvider => new GithubInstallationBuilder(
                                            serviceProvider.GetRequiredService<IGameSetupService>(),
                                            serviceProvider.GetRequiredService<IImyaSetupService>(),
                                            serviceProvider.GetRequiredService<ITextManager>(),
                                            serviceProvider.GetRequiredService<IReleaseAssetStrategy>(),
                                            serviceProvider.GetRequiredService<IModImageStrategy>()));
                    services.AddSingleton<ZipInstallationBuilder>(serviceProvider => new ZipInstallationBuilder(
                                            serviceProvider.GetRequiredService<IGameSetupService>(),
                                            serviceProvider.GetRequiredService<IImyaSetupService>(),
                                            serviceProvider.GetRequiredService<ITextManager>()));

                    //application
                    services.AddSingleton<ModActivationView>();
                    services.AddSingleton<GithubBrowserView>();
                    services.AddSingleton<InstallationView>();
                    services.AddSingleton<ModTweakerView>();
                    services.AddSingleton<SettingsView>();
                    services.AddSingleton<PopupCreator>();
                    services.AddSingleton<MainWindow>();
                    services.AddSingleton<IMainViewController, MainViewController>();
                    services.AddSingleton<IAuthenticationController, AuthenticationController>();
                })
                .Build();

            var textManager = AppHost.Services.GetRequiredService<ITextManager>();
            textManager.LoadLanguageFile(Settings.Default.LanguageFilePath);

            var gameSetup = AppHost.Services.GetRequiredService<IGameSetupService>();
            gameSetup.SetGamePath(Settings.Default.GameRootPath, true);
            gameSetup.SetModDirectoryName(Settings.Default.ModDirectoryName);

            //subscribe the global mod collection to the gamesetup
            var globalMods = AppHost.Services.GetRequiredService<ModCollection>();
            gameSetup.GameRootPathChanged += globalMods.OnModPathChanged;
            gameSetup.ModDirectoryNameChanged += globalMods.OnModPathChanged;
            //check if this can be moved to OnStartup
            Task.Run(() => globalMods.LoadModsAsync());
            globalMods.Hooks.AddHook(AppHost.Services.GetRequiredService<ModContentValidator>());
            globalMods.Hooks.AddHook(AppHost.Services.GetRequiredService<ModCompatibilityValidator>());
            globalMods.Hooks.AddHook(AppHost.Services.GetRequiredService<CyclicDependencyValidator>());
            globalMods.Hooks.AddHook(AppHost.Services.GetRequiredService<ModDependencyValidator>());
            globalMods.Hooks.AddHook(AppHost.Services.GetRequiredService<ModReplacementValidator>());
            globalMods.Hooks.AddHook(AppHost.Services.GetRequiredService<RemovedModValidator>());

            var appSettings = AppHost.Services.GetRequiredService<IAppSettings>();
            var installationService = AppHost.Services.GetRequiredService<IInstallationService>();

            if (appSettings.UseRateLimiting)
                installationService.DownloadConfig.MaximumBytesPerSecond = appSettings.DownloadRateLimit;
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await AppHost.StartAsync();
            var startupForm = AppHost.Services.GetRequiredService<MainWindow>();
            startupForm.Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await AppHost.StopAsync();
            base.OnExit(e);
        }

        /*
        public App()
        {
            // load localized text first
            var text = TextManager.Instance;
            text.LoadLanguageFile(Settings.Default.LanguageFilePath);


            var gameSetup = GameSetupManager.Instance;
            //gameSetup.SetDownloadDirectory(Settings.Default.DownloadDir);
            gameSetup.SetGamePath(Settings.Default.GameRootPath, true);
            gameSetup.SetModDirectoryName(Settings.Default.ModDirectoryName);
            var appSettings = new AppSettings();

            GithubClientProvider.Authenticator = new DeviceFlowAuthenticator();

            // init global mods
            ModCollection.Global = new ModCollection(gameSetup.GetModDirectory(), normalize: true, loadImages: true);
            _hooks = new(ModCollection.Global);
            Task.Run(() => ModCollection.Global.LoadModsAsync());

            if(AppSettings.Instance.UseRateLimiting)
                InstallationManager.Instance.DownloadConfig.MaximumBytesPerSecond = AppSettings.Instance.DownloadRateLimit;
        }
        */
    }
}
