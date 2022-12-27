using Imya.GithubIntegration;
using Imya.GithubIntegration.Download;
using Imya.Utils;

namespace Imya.Models.Installation
{
    public class GithubInstallationBuilder
    {
        private GithubInstallation _installation;
        private bool _ignoreRepoInfoForUrl = false;

        private GithubRepoInfo? _repoInfoToInstall;
        private String _url;

        private long? _download_size;

        private GithubInstallationBuilder() { }

        public static GithubInstallationBuilder Create() => new GithubInstallationBuilder();

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
            if (GameSetupManager.Instance.GameRootPath is null)
                throw new Exception("No Game Path set!");

            if (_repoInfoToInstall is null)
                throw new Exception("Please set a repo info before building");

            if (!_ignoreRepoInfoForUrl)
            {
                var releaseasset = await _repoInfoToInstall.GetReleaseAssetAsync();
                if (releaseasset is null)
                    throw new InstallationException($"Could not fetch any release for {_repoInfoToInstall}");
                _url = releaseasset.BrowserDownloadUrl;
                _download_size = releaseasset.Size;
            }

            var installationGuid = Guid.NewGuid().ToString();
            var sourceFilepath = Path.Combine(ImyaSetupManager.Instance.DownloadDirectoryPath, installationGuid + ".zip");

            var header = $"{_repoInfoToInstall.Owner}/{_repoInfoToInstall.Name}";
            var additional = $"{_repoInfoToInstall.ReleaseID}";

            var id = _repoInfoToInstall.GetID();

            _installation = new GithubInstallation()
            {
                RepositoryToInstall = _repoInfoToInstall!,
                SourceFilepath = sourceFilepath,
                DownloadTargetFilename = sourceFilepath,
                UnpackTargetPath = Path.Combine(ImyaSetupManager.Instance.UnpackDirectoryPath, installationGuid),
                DownloadUrl = _url,
                DownloadSize = _download_size,
                HeaderText = new SimpleText(header),
                AdditionalText = new SimpleText(additional),
                ID = id,
                Status = InstallationStatus.NotStarted
            };    
            return _installation;
        }
    }
}
