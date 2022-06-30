using System.IO.Compression;
using Imya.GithubIntegration;
using Imya.Models;
using Imya.Models.Installation;
using Imya.Models.Options;

namespace Imya.Utils
{
    public class ModloaderInstallation : GithubInstallation
    {
        public static GithubRepoInfo ModloaderRepository { get; } = new GithubRepoInfo() { Name = "anno1800-mod-loader", Owner = "xforce", AssetName="loader.zip"};

        public ModloaderInstallationOptions ModloaderInstallationOptions { get; } = new ModloaderInstallationOptions();

        internal ModloaderInstallation() : base(ModloaderRepository)
        {
            HeaderText = TextManager.Instance.GetText("INSTALLATION_HEADER_LOADER");
        }

        public override Task<IInstallation> Setup()
        {
            return Task.Run(async () =>
            {
                await DownloadAsync();
                Unpack();
                return this as IInstallation;
            }
            );
        }

        public override Task Finalize()
        {
            if (TargetFilename is not String target) return Task.CompletedTask;
           
            Status = GithubInstallationStatus.MovingFiles;
            return Task.Run(() =>
            {
                foreach (string absFile in Directory.GetFiles(TargetFilename))
                {
                    string relFile = Path.GetFileName(absFile);
                    File.Move(Path.Combine(TargetFilename, relFile), Path.Combine(GameSetup.ExecutableDir, relFile), true);
                }
                CleanUp();
            }
            );
        }

        protected void Unpack()
        {
            if (!DownloadResult.DownloadSuccessful) return;
            String DownloadFilename = DownloadResult.DownloadDestination;
            TargetFilename = Path.Combine(ModloaderInstallationOptions.UnpackDirectory, Path.GetFileNameWithoutExtension(DownloadFilename));

            Status = GithubInstallationStatus.Unpacking;
            ZipFile.ExtractToDirectory(DownloadFilename, TargetFilename, true);
        }

        public override void CleanUp()
        {
            if(TargetFilename is String target && Directory.Exists(target))
                Directory.Delete(target);
            if (DownloadResult.DownloadSuccessful && File.Exists(DownloadResult.DownloadDestination))
                File.Delete(DownloadResult.DownloadDestination);
        }
    }
}
