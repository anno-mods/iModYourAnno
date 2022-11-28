using Imya.GithubIntegration;
using Imya.GithubIntegration.Download;
using Imya.GithubIntegration.StaticData;
using Imya.Models.Installation;
using Imya.Models.Options;
using Imya.Utils;

internal class GithubDevTester
{
    internal async static Task DownloadSpice()
    {
        GithubDownloader Downloader = new GithubDownloader( new GithubDownloaderOptions() { DownloadDirectory = "fuck"});

        File.Delete("fuck/loader.zip");
        await Downloader.DownloadRepoInfoAsync(StaticNameGithubRepoInfoFactory.CreateWithStaticName("Spice-it-Up", "anno-mods", "Spice-it-Up.zip"), new ModloaderInstallation());

        var file = new FileInfo("fuck/Spice-it-Up.zip");
        Console.WriteLine($"Download Success: { file.Exists && file.Length != 0 }");
    }
}