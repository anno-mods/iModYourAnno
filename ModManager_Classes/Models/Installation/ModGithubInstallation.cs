using Imya.GithubIntegration;
using Imya.Models.Options;
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

        private String DownloadDestination;

        internal ModGithubInstallation(GithubRepoInfo repoInfo, ModInstallationOptions options) : base(repoInfo)
        {
            HeaderText = TextManager.Instance.GetText("INSTALLATION_HEADER_MOD_GIT");
            AdditionalText = new SimpleText(repoInfo.ToString());
            Options = options;

            //sanitize releaseid just in case
            string CleanReleaseID = String.Concat(RepositoryToInstall.ReleaseID.Split(Path.GetInvalidFileNameChars()));

            //preemtively set this for later error handling
            DownloadDestination = Path.Combine(ImyaSetupManager.Instance.DownloadDirectoryPath, CleanReleaseID);
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
                {
                    DownloadDestination = DownloadResult.DownloadDestination;
                    Status = GithubInstallationStatus.MovingFiles;
                    Result = await ModCollectionLoader.ExtractZipAsync(DownloadResult.DownloadDestination,
                        Options.UnpackDirectory,
                        this);
                }
                return this as IInstallation;
            }
            );
        }

        public override void CleanUp()
        {
            if(DownloadDestination is String dest && File.Exists(dest))
                File.Delete(dest);
        }

    }
}
