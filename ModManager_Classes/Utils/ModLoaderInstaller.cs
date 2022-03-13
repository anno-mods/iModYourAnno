using System.IO.Compression;
using Imya.GithubIntegration;

namespace Imya.Utils
{
    /// <summary>
    /// Check and install mod loader.
    /// </summary>
    public class ModLoaderInstaller : Imya.Models.NotifyPropertyChanged.PropertyChangedNotifier
    {
        public string DownloadDirectory { get; private set; }
        public bool IsInstalled { get; private set; } = false;

        readonly GithubDownloader? GithubDownloader;
        readonly GameSetupManager GameSetup = GameSetupManager.Instance;

        public ModLoaderInstaller(string gamePath, string downloadDirectory)
        {
            DownloadDirectory = downloadDirectory;
            GithubDownloader = new GithubDownloader(downloadDirectory);
            if (gamePath == null)
                return;

            IsInstalled = CheckInstallation();
        }

        /// <summary>
        /// Download and install mod loader from GitHub.
        /// </summary>
        public async Task InstallAsync()
        {
            if (GameSetup.ExecutableDir == null)
            {
                // TODO disable install UI based on game path setting
                Console.WriteLine($"Game path is not set yet.");
                return;
            }

            var modloaderRepo = new GithubRepoInfo() { Name = "anno1800-mod-loader", Owner = "xforce" };
            string downloadResult = await GithubDownloader!.DownloadReleaseAsync(modloaderRepo, "loader.zip");

            string target = Path.Combine(Path.GetDirectoryName(downloadResult)??"", Path.GetFileNameWithoutExtension(downloadResult));

            ZipFile.ExtractToDirectory(downloadResult, target, true);
           
            foreach (string absFile in Directory.GetFiles(target))
            { 
                string relFile = Path.GetFileName(absFile);
                File.Move(Path.Combine(target, relFile), Path.Combine(GameSetup.ExecutableDir, relFile), true);
            }

            Directory.Delete(target);
            IsInstalled = CheckInstallation();
        }

        /// <summary>
        /// Check if there's an updated mod loader on GitHub.
        /// </summary>
        public async Task CheckForUpdatesAsync()
        {
            // TODO
            // - check release tag against local version
            // - optional: crawl release notes for latest "Game Update ?\d\d(\.\d)?" or "GU ?\d\d(\.\d)?" 
            await Task.Run(() => { });
        }

        private bool CheckInstallation()
        {
            if (GameSetup.ExecutableDir == null) return false;

            var ubiPython = Path.Combine(GameSetup.ExecutableDir, "python35_ubi.dll");
            var python = Path.Combine(GameSetup.ExecutableDir, "python35.dll");
            var executable = GameSetup.ExecutablePath;
            if (File.Exists(ubiPython) && File.Exists(python) && File.Exists(executable))
            {
                // when the executable is a lot newer than ubiPython, then it either got repaired or updated
                // chances are you need an update, but there's no concrete action that you could do here
                // IsPotentiallyOutdated = File.GetLastWriteTimeUtc(executable) > File.GetLastWriteTimeUtc(ubiPython).AddHours(1);

                // mod loader has python and ubiPython at roughly the same time
                // in case python is newer chances are it got repaired.
                // consider it to be not installed in that case.
                // note: will give false results if you repair right before you download a super fresh mod loader release
                // TODO add hash-based check. remove the false result, but is only applicable when downloaded via imya
                return File.GetLastWriteTimeUtc(python) <= File.GetLastWriteTimeUtc(ubiPython).AddMinutes(20);
            }

            return false;
        }
    }
}
