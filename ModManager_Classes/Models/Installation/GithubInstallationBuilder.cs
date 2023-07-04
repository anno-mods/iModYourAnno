using Imya.GithubIntegration;
using Imya.GithubIntegration.Download;
using Imya.Models.Installation.Interfaces;
using Imya.Services;
using Imya.Services.Interfaces;
using Imya.Texts;
using Microsoft.Extensions.DependencyInjection;

namespace Imya.Models.Installation
{
    public class GithubInstallationBuilderFactory : IGithubInstallationBuilderFactory
    {
        public IServiceProvider _serviceProvider; 
        public GithubInstallationBuilderFactory(IServiceProvider serviceProvider)
        { 
            _serviceProvider = serviceProvider;
        }

        public GithubInstallationBuilder Create() => _serviceProvider.GetRequiredService<GithubInstallationBuilder>();
    }

    public class GithubInstallationBuilder
    {
        private GithubInstallation _installation;
        private bool _ignoreRepoInfoForUrl = false;

        private GithubRepoInfo? _repoInfoToInstall;
        private String _url;

        private long? _download_size;

        private readonly IGameSetupService _gameSetupService;
        private readonly IImyaSetupService _imyaSetupService;
        private readonly ITextManager _textManager;
        private readonly IReleaseAssetStrategy _releaseAssetStrategy;
        private readonly IModImageStrategy _imageStrategy;

        public GithubInstallationBuilder(
            IGameSetupService gameSetupService,
            IImyaSetupService imyaSetupService,
            ITextManager textManager,
            IReleaseAssetStrategy releaseAssetStrategy,
            IModImageStrategy imageStragegy) 
        {
            _gameSetupService = gameSetupService;
            _imyaSetupService = imyaSetupService;
            _textManager = textManager;
            _releaseAssetStrategy = releaseAssetStrategy;
            _imageStrategy = imageStragegy;
        }

        public GithubInstallationBuilder WithRepoInfo(GithubRepoInfo repoInfo) 
        {
            _repoInfoToInstall = repoInfo;
            return this;
        }

        public GithubInstallationBuilder WithUrl(String url, bool force = false)
        {
            _url = url;
            _ignoreRepoInfoForUrl = force;
            return this;
        }


        /// <summary>
        /// Set this on your builder if you want to use <see cref="WithUrl(string)" /> to overwrite the Url that is set by <see cref="BuildAsync"/>
        /// </summary>
        /// <returns></returns>
        public GithubInstallationBuilder UseModloaderInstallFlow(bool use)
        {
            if (use)
            {
                _ignoreRepoInfoForUrl = true;
                _installation.UseModloaderInstallFlow = true;
            }
            return this;
        }

        public async Task<GithubInstallation> BuildAsync()
        {
            if (_gameSetupService.GameRootPath is null)
                throw new Exception("No Game Path set!");

            if (_repoInfoToInstall is null)
                throw new Exception("Please set a repo info before building");

            if (!_ignoreRepoInfoForUrl)
            {
                var releaseasset = await _releaseAssetStrategy.GetReleaseAssetAsync(_repoInfoToInstall);
                if (releaseasset is null)
                    throw new InstallationException($"Could not fetch any release for {_repoInfoToInstall}");
                _url = releaseasset.BrowserDownloadUrl;
                _download_size = releaseasset.Size;
            }

            var installationGuid = Guid.NewGuid().ToString();
            var sourceFilepath = Path.Combine(_imyaSetupService.DownloadDirectoryPath, installationGuid + ".zip");

            var header = $"{_repoInfoToInstall.Owner}/{_repoInfoToInstall.Name}";
            var additional = $"{_repoInfoToInstall.ReleaseID}";

            var id = _repoInfoToInstall.GetID();
            var url = await _imageStrategy.GetImageUrlAsync(_repoInfoToInstall);

            _installation = new GithubInstallation()
            {
                RepositoryToInstall = _repoInfoToInstall!,
                SourceFilepath = sourceFilepath,
                DownloadTargetFilename = sourceFilepath,
                UnpackTargetPath = Path.Combine(_imyaSetupService.UnpackDirectoryPath, installationGuid),
                DownloadUrl = _url,
                DownloadSize = _download_size,
                AdditionalText = new SimpleText(additional),
                ID = id,
                Status = InstallationStatus.NotStarted,
                ImageUrl = url,
                CancellationTokenSource = new CancellationTokenSource(),
                HeaderText = _textManager.GetText("INSTALLATION_HEADER_MOD")
        };    
            return _installation;
        }
    }
}
