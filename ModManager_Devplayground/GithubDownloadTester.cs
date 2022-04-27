using Imya.GithubIntegration;
using Imya.Models.Installation;
using Imya.Utils;

internal class GithubDevTester
{
    internal async static Task DownloadSpice()
    {
        GithubDownloader Downloader = new GithubDownloader("fuck");

        File.Delete("fuck/loader.zip");

        await Downloader.DownloadReleaseAsync(new GithubRepoInfo { Name = "Spice-it-Up", Owner = "anno-mods" }, "Spice-it-Up.zip", new ModloaderInstallation());

        var file = new FileInfo("fuck/Spice-it-Up.zip");
        Console.WriteLine($"Download Success: { file.Exists && file.Length != 0 }");
    }

    internal async static Task DownloadModloader2()
    {
        GameSetupManager.Instance.SetDownloadDirectory("imya_temp");
        Imya.Utils.ModloaderInstallation installation = new Imya.Utils.ModloaderInstallation("");
        await installation.InstallAsync();
    }

    internal class ModloaderInstallation : Installation
    {

    }
}