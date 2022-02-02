using Imya.GithubIntegration;

internal class GithubDevTester
{
    internal async static void DownloadModloader()
    {
        GithubDownloader Downloader = new GithubDownloader("fuck");

        File.Delete("fuck/loader.zip");

        await Downloader.DownloadReleaseAsync(new GithubRepoInfo { Name = "anno1800-mod-loader", Owner = "xforce" }, "loader.zip");

        var file = new FileInfo("fuck/loader.zip");
        Console.WriteLine($"Download Success: { file.Exists && file.Length != 0 }");
    }
}