using Anno.EasyMod.DI;
using Anno.EasyMod.Utils;
using Anno.Utils;
using Imya.GithubIntegration;
using Imya.GithubIntegration.RepositoryInformation;
using Imya.GithubIntegration.StaticData;
using Imya.Models.Attributes.Factories;
using Imya.Models.Attributes.Interfaces;
using Imya.Models.Cache;
using Imya.Models.GameLauncher;
using Imya.Models.Installation;
using Imya.Models.Installation.Interfaces;
using Imya.Models.ModTweaker.DataModel.Storage;
using Imya.Models.ModTweaker.IO;
using Imya.Models.Options;
using Imya.Services;
using Imya.Services.Interfaces;
using Imya.Texts;
using Imya.UI.Components;
using Imya.UI.Models;
using Imya.UI.Properties;
using Imya.UI.Utils;
using Imya.UI.ValueConverters;
using Imya.UI.Views;
using Imya.Utils;
using Imya.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Octokit;
using Serilog;
using Serilog.Sinks.RichTextBox.Themes;
using System;
using System.Windows;

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
                    services.ConfigureEasyMod();
                    //services.ConfigureModio(new Modio.Client(new Modio.Credentials("apiKey", "oauthtoken")));
                    services.AddSingleton<ITextManager, TextManager>();
                    services.AddSingleton<IGameSetupService, GameSetupService>();
                    var gameSetup = services.BuildServiceProvider().GetRequiredService<IGameSetupService>();
                    gameSetup.SetGamePath(Settings.Default.GameRootPath, true);
                    gameSetup.SetModDirectoryName(Settings.Default.ModDirectoryName);

                    services.AddSingleton<IImyaSetupService, ImyaSetupService>();
                    services.AddSingleton<IGameFilesService, GameFileService>();
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

                    //value conversion
                    services.AddTransient<DlcTextConverter>();
                    services.AddTransient<FilenameValidationConverter>();
                    services.AddTransient<FilepathToImageConverter>();
                    services.AddTransient<DlcIconConverter>();

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
                    services.AddTransient<DlcOwnershipAttributeFactory>();
                    services.AddSingleton<DlcOwnershipValidator>();
                    //need to register dlc ownership 

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
                    services.AddSingleton<System.Windows.Controls.RichTextBox>(sp => {
                        return new System.Windows.Controls.RichTextBox();
                    });
                    services.AddSingleton<RedirectOutput>();

                    services.AddSingleton<SelfUpdater>();
                })
                .ConfigureLogging(builder => 
                {
                    builder.ClearProviders();
                    builder.AddSerilog();
                    builder.SetMinimumLevel(LogLevel.Trace);
                })
                .UseSerilog((hostingContext, services, loggerConfiguration)
                    => loggerConfiguration
                        .Enrich.FromLogContext()
                        .WriteTo.RichTextBox(
                                services.GetService<System.Windows.Controls.RichTextBox>(), 
                                theme: RichTextBoxConsoleTheme.None,
                                outputTemplate: "[{Timestamp:HH:mm:ss}] {Message:lj}{NewLine}{Exception}"
                            )
                        )
                .Build();

            var textManager = AppHost.Services.GetRequiredService<ITextManager>();
            textManager.LoadLanguageFile(Settings.Default.LanguageFilePath);

            var gameSetup = AppHost.Services.GetRequiredService<IGameSetupService>();
            var settings = AppHost.Services.GetRequiredService<IAppSettings>();
            settings.GamePath = Settings.Default.GameRootPath;
            settings.ModDirectoryName = Settings.Default.ModDirectoryName; 
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            var gameSetup = AppHost.Services.GetRequiredService<IGameSetupService>();
            var imyaSetup = AppHost.Services.GetRequiredService<IImyaSetupService>();
            var hooks = AppHost.Services.GetRequiredService<ModCollectionHooks>();

            var collection = await AppHost.Services
                .GetRequiredService<CollectionBuilder>()
                .AddFromLocalSource(gameSetup.GetModDirectory())
                //.AddModio()
                .BuildAsync();
            imyaSetup.GlobalModCollection = collection;

            //subscribe the global mod collection to the gamesetup
            //check if this can be moved to OnStartup
            hooks.HookTo(imyaSetup.GlobalModCollection);
            hooks.AddHook(AppHost.Services.GetRequiredService<ModContentValidator>());
            hooks.AddHook(AppHost.Services.GetRequiredService<ModCompatibilityValidator>());
            hooks.AddHook(AppHost.Services.GetRequiredService<CyclicDependencyValidator>());
            hooks.AddHook(AppHost.Services.GetRequiredService<ModDependencyValidator>());
            hooks.AddHook(AppHost.Services.GetRequiredService<ModReplacementValidator>());
            hooks.AddHook(AppHost.Services.GetRequiredService<RemovedModValidator>());
            hooks.AddHook(AppHost.Services.GetRequiredService<TweakValidator>());
            hooks.AddHook(AppHost.Services.GetRequiredService<DlcOwnershipValidator>());
            hooks.HookTo(AppHost.Services.GetRequiredService<IAppSettings>());

            var appSettings = AppHost.Services.GetRequiredService<IAppSettings>();
            appSettings.Initialize();
            var installationService = AppHost.Services.GetRequiredService<IInstallationService>();

            var gamefileService = AppHost.Services.GetRequiredService<IGameFilesService>();
            await gamefileService.LoadAsync(); 

            if (appSettings.UseRateLimiting)
                installationService.DownloadConfig.MaximumBytesPerSecond = appSettings.DownloadRateLimit;

            await AppHost.StartAsync();

            //hacky converters with dependencyinjection....
            Resources.Add("DlcTextConverter", AppHost.Services.GetRequiredService<DlcTextConverter>());
            Resources.Add("FilepathToImageConverter", AppHost.Services.GetRequiredService<FilepathToImageConverter>());
            Resources.Add("FilenameValidationConverter", AppHost.Services.GetRequiredService<FilenameValidationConverter>());
            Resources.Add("DlcIconConverter", AppHost.Services.GetRequiredService<DlcIconConverter>());

            var startupForm = AppHost.Services.GetRequiredService<MainWindow>();
            AppHost.Services.GetRequiredService<System.Windows.Controls.RichTextBox>()
                .Style = (Style)FindResource("IMYA_RICHTEXTBOX");
            var redirectedOut = AppHost.Services.GetRequiredService<RedirectOutput>();
            Console.SetOut(redirectedOut);
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
