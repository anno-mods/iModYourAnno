using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using Octokit;

namespace Anno.Utils
{
    internal class SelfUpdater
    {
        public static void CheckForUpdate(GitHubClient client, string owner, string repo)
        {
            Task.Run(async () =>
            {
                var availableUpdate = await IsUpdateAvailableAsync(client, owner, repo);
                if (availableUpdate is not null)
                {
                    var answer = MessageBox.Show($"iModYourAnno {availableUpdate.TagName} is available.\n\nDo you want to download it now?",
                        "Update Available", MessageBoxButton.YesNo);
                    if (answer == MessageBoxResult.Yes)
                    {
                        OpenReleasePage(availableUpdate);
                    }
                }
            });
        }

        public async static Task<Release?> IsUpdateAvailableAsync(GitHubClient client, string owner, string repo)
        {
            var latest = await client.Repository.Release.GetLatest(owner, repo);
            if (latest is null) return null;
            
            Console.WriteLine(
                "The latest release is tagged at {0} and is named {1}",
                latest.TagName,
                latest.Name);

            var latestVersion = new Version(latest.TagName[1..]);
            var currentVersion = GetCurrentVersion();

            return (latestVersion > currentVersion) ? latest : null;
        }

        public static void OpenReleasePage(Release release)
        {
            var info = new ProcessStartInfo(release.HtmlUrl)
            {
                UseShellExecute = true,
            };
            Process.Start(info);
        }

        public static Version GetCurrentVersion()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            return new Version((fvi.FileVersion ?? "v0.0")[1..].Split('-')[0]);
        }
    }
}
