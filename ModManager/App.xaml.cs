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
using Imya.Models.ModTweaker.IO;
using Imya.UI.Components;
using Imya.Models.ModMetadata;
using Octokit;
using Imya.GithubIntegration.JsonData;
using Imya.GithubIntegration.RepositoryInformation;
using Imya.UI.ValueConverters;
using Anno.Utils;

namespace Imya.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        public static IHost AppHost { get; private set; }

        public App()
        {
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    //services
                    services.AddSingleton<ITextManager, TextManager>();
                    services.AddSingleton<IGameSetupService, GameSetupService>();
                    var gameSetup = services.BuildServiceProvider().GetRequiredService<IGameSetupService>();
                    gameSetup.SetGamePath(Settings.Default.GameRootPath, true);
                    gameSetup.SetModDirectoryName(Settings.Default.ModDirectoryName);

                    services.AddSingleton<IImyaSetupService, ImyaSetupService>();
                    services.AddTransient<ICyclicDependencyAttributeFactory, CyclicDependencyAttributeFactory>();
                    services.AddTransient<IMissingModinfoAttributeFactory, MissingModinfoAttributeFactory>();
                    services.AddTransient<IModCompabilityAttributeFactory, ModCompabilityAttributeFactory>();
                    services.AddTransient<IModDependencyIssueAttributeFactory, ModDependencyIssueAttributeFactory>();
                    services.AddTransient<IModReplacedByAttributeFactory, ModReplacedByAttributeFactory>();
                    services.AddTransient<IModStatusAttributeFactory, ModStatusAttributeFactory>();
                    services.AddTransient<IRemovedFolderAttributeFactory, RemovedFolderAttributeFactory>();
                    services.AddTransient<ITweakedAttributeFactory, TweakedAttributeFactory>();
                    services.AddTransient<IContentInSubfolderAttributeFactory, ContentInSubfolderAttributeFactory>();
                    services.AddTransient<IModAccessIssueAttributeFactory, ModAccessIssueAttributeFactory>();

                    services.AddSingleton<LocalizedModinfoFactory>();
                    services.AddSingleton<IModFactory, ModFactory>();
                    services.AddSingleton<IModCollectionFactory, ModCollectionFactory>();
                    services.AddTransient<ModTweaksStorageModelLoader>();
                    services.AddSingleton<ITweakRepository, TweakRepository>();
                    services.AddSingleton<ModTweaksLoader>();
                    services.AddSingleton<ModTweaksExporter>();
                    services.AddSingleton<ModCollectionHooks>(serviceProvider => new ModCollectionHooks());

                    services.AddSingleton<CyclicDependencyAttributeFactory>();

                    services.AddSingleton<IInstallationService, InstallationService>();
                    services.AddSingleton<IAppSettings, AppSettings>();
                    services.AddSingleton<ITweakService, TweakService>();

                    //github integration
                    var githubClient = new GitHubClient(new ProductHeaderValue("iModYourAnno"));
                    services.AddSingleton<Octokit.IGitHubClient, Octokit.GitHubClient>(x => githubClient);
                    services.AddTransient<IReadmeStrategy, StaticFilenameReadmeStrategy>();
                    services.AddTransient<IReleaseAssetStrategy, StaticNameReleaseAssetStrategy>();
                    services.AddTransient<IModImageStrategy, StaticFilepathImageStrategy>();
                    services.AddSingleton<IAuthenticator, DeviceFlowAuthenticator>();

                    //tweaks

                    //game launcher
                    services.AddSingleton<IGameLauncherFactory, GameLauncherFactory>();
                    services.AddSingleton<SteamGameLauncher>(serviceProvider => new SteamGameLauncher(
                                            serviceProvider.GetRequiredService<IGameSetupService>()));
                    services.AddSingleton<StandardGameLauncher>(serviceProvider => new StandardGameLauncher(
                                            serviceProvider.GetRequiredService<IGameSetupService>()));
                    //hooks
                    services.AddSingleton<CyclicDependencyValidator>(serviceProvider => new CyclicDependencyValidator(
                                            serviceProvider.GetRequiredService<CyclicDependencyAttributeFactory>()));
                    services.AddSingleton<ModCompatibilityValidator>();
                    services.AddSingleton<ModContentValidator>();
                    services.AddSingleton<ModDependencyValidator>();
                    services.AddSingleton<ModReplacementValidator>();
                    services.AddSingleton<RemovedModValidator>();
                    services.AddSingleton<TweakValidator>();

                    //caching
                    services.AddScoped<ICache<GithubRepoInfo, String>, TimedCache<GithubRepoInfo, String>>();

                    //installation
                    services.AddScoped<IModInstallationOptions, ModInstallationOptions>();
                    services.AddScoped<IGithubDownloaderOptions, GithubDownloaderOptions>();
                    services.AddScoped<IModloaderInstallationOptions, ModloaderInstallationOptions>();
                    services.AddScoped<IGithubInstallationBuilderFactory, GithubInstallationBuilderFactory>();
                    services.AddScoped<IZipInstallationBuilderFactory, ZipInstallationBuilderFactory>();

                    services.AddScoped<IRepositoryProvider, RepositoryProvider>();
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

                    services.AddSingleton<IProfilesService, ProfilesService>();

                    //application
                    services.AddTransient<ModList>();
                    services.AddTransient<ModTweaker>();
                    services.AddTransient<Dashboard>();
                    services.AddTransient<ConsoleLog>();
                    services.AddTransient<ModDescriptionDisplay>();
                    services.AddSingleton<ModActivationView>();
                    services.AddSingleton<GithubBrowserView>();
                    services.AddSingleton<InstallationView>();
                    services.AddSingleton<ModinfoCreatorView>();
                    services.AddSingleton<ModTweakerView>();
                    services.AddSingleton<SettingsView>();
                    services.AddSingleton<PopupCreator>();
                    services.AddSingleton<MainWindow>();
                    services.AddSingleton<IMainViewController, MainViewController>();
                    services.AddSingleton<IAuthenticationController, AuthenticationController>();

                    services.AddTransient<DlcTextConverter>();
                    services.AddTransient<FilenameValidationConverter>();
                    services.AddSingleton<SelfUpdater>();
                })
                .Build();


            var gameSetup = AppHost.Services.GetRequiredService<IGameSetupService>();
            gameSetup.SetGamePath(Settings.Default.GameRootPath, true);
            gameSetup.SetModDirectoryName(Settings.Default.ModDirectoryName);

            var factory = AppHost.Services.GetRequiredService<IModCollectionFactory>();
            var collection = factory.Get(gameSetup.GetModDirectory(), normalize: true, loadImages: true);
            var imyaSetup = AppHost.Services.GetRequiredService<IImyaSetupService>();
            imyaSetup.GlobalModCollection = collection; 

            //subscribe the global mod collection to the gamesetup
            var textManager = AppHost.Services.GetRequiredService<ITextManager>();
            textManager.LoadLanguageFile(Settings.Default.LanguageFilePath);
            //check if this can be moved to OnStartup
            var globalMods = AppHost.Services.GetRequiredService<IImyaSetupService>().GlobalModCollection;
            globalMods.Hooks.AddHook(AppHost.Services.GetRequiredService<ModContentValidator>());
            globalMods.Hooks.AddHook(AppHost.Services.GetRequiredService<ModCompatibilityValidator>());
            globalMods.Hooks.AddHook(AppHost.Services.GetRequiredService<CyclicDependencyValidator>());
            globalMods.Hooks.AddHook(AppHost.Services.GetRequiredService<ModDependencyValidator>());
            globalMods.Hooks.AddHook(AppHost.Services.GetRequiredService<ModReplacementValidator>());
            globalMods.Hooks.AddHook(AppHost.Services.GetRequiredService<RemovedModValidator>());
            globalMods.Hooks.AddHook(AppHost.Services.GetRequiredService<TweakValidator>());


        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            var globalMods = AppHost.Services.GetRequiredService<IImyaSetupService>().GlobalModCollection;
            await globalMods.LoadModsAsync();

            var appSettings = AppHost.Services.GetRequiredService<IAppSettings>();
            appSettings.Initialize();
            var installationService = AppHost.Services.GetRequiredService<IInstallationService>();

            if (appSettings.UseRateLimiting)
                installationService.DownloadConfig.MaximumBytesPerSecond = appSettings.DownloadRateLimit;

            await AppHost.StartAsync();

            //hacky converters with dependencyinjection....
            Resources.Add("DlcTextConverter", AppHost.Services.GetRequiredService<DlcTextConverter>());
            Resources.Add("FilenameValidationConverter", AppHost.Services.GetRequiredService<FilenameValidationConverter>());

            var startupForm = AppHost.Services.GetRequiredService<MainWindow>();
            startupForm.Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await AppHost.StopAsync();
            base.OnExit(e);
        }
    }
}
