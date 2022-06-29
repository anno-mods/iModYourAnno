using Imya.GithubIntegration;
using Imya.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.Installation
{
    internal class ModGithubInstallation : GithubInstallation, IModInstallation
    {
        public ModCollection? Result { get; set; }
        public ModInstallationOptions Options { get; init; }

        public async Task<String?> GetRepositoryDescription()
        {
            return await GithubDownloader.FetchDescriptionAsync(RepositoryToInstall);
        }

        internal ModGithubInstallation(GithubRepoInfo repoInfo, ModInstallationOptions options) : base(repoInfo)
        {
            HeaderText = TextManager.Instance.GetText("INSTALLATION_HEADER_MOD_GIT");
            AdditionalText = new SimpleText($"{repoInfo.Owner}/{repoInfo.Name}: {repoInfo.AssetName}");
            Options = options;
        }

        public override Task Finalize()
        {
            Status = GithubInstallationStatus.MovingFiles;
            return this.RunMoveInto();
        }

        public override Task<IInstallation> Setup()
        {
            return Task.Run(async () =>
            {
                await DownloadAsync();
                if (DownloadResult.DownloadSuccessful)
                    Result = await ModCollectionLoader.ExtractZipAsync(DownloadResult.DownloadDestination,
                        UnpackDirectory,
                        this);
                return this as IInstallation;
            }
            );
        }

        public override void CleanUp()
        { 
        
        }

    }
}
