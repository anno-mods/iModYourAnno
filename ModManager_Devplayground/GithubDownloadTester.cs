using Imya.GithubIntegration;
using Imya.Models.Installation;
using Imya.Utils;

internal class GithubDevTester
{
    internal async static Task DownloadSpice()
    {
        GithubDownloader Downloader = new GithubDownloader("fuck");

        File.Delete("fuck/loader.zip");

        await Downloader.DownloadRepoInfoAsync(new GithubRepoInfo { Name = "Spice-it-Up", Owner = "anno-mods", AssetName = "Spice-it-Up.zip"}, new ModloaderInstallation());

        var file = new FileInfo("fuck/Spice-it-Up.zip");
        Console.WriteLine($"Download Success: { file.Exists && file.Length != 0 }");
    }

    internal async static Task DownloadModloader2()
    {
        GameSetupManager.Instance.SetGamePath("dummygamepath");
        var Installer = new Installer();

        var installation = await Installer.CreateModloaderInstallationTask();
        await installation.Finalize();
    }

    internal class ModloaderInstallation : Installation
    {
        public override void CleanUp()
        {
            throw new NotImplementedException();
        }

        public override Task Finalize()
        {
            throw new NotImplementedException();
        }

        public override Task<IInstallation> Setup()
        {
            throw new NotImplementedException();
        }
    }
}