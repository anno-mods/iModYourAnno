using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imya.GithubIntegration;

namespace Imya.Utils
{
    //Handles installation of Mods and Modloader
    public class InstallationManager
    {
        GithubDownloader GithubDownloader;  
        public static InstallationManager Instance;
        GameSetupManager GameSetupManager = GameSetupManager.Instance;

        public InstallationManager(String s)
        {
            GithubDownloader = new GithubDownloader(s);
            Instance ??= this;
        }

        public async Task InstallModLoaderAsync()
        {
            if (GameSetupManager.ExecutableDir == null)
            {
                // TODO properly tell'em
                Console.WriteLine($"Game path is not set yet.");
                return;
            }

            GithubRepoInfo modloaderRepo = new GithubRepoInfo() { Name = "anno1800-mod-loader", Owner = "xforce" };
            String DownloadResult = await GithubDownloader.DownloadReleaseAsync(modloaderRepo, "loader.zip");

            String target = Path.ChangeExtension(DownloadResult, "");
            target = target.Substring(0, target.Length - 1);

            //move the files to bin/win64 
            String Win64Dir = GameSetupManager.ExecutableDir;

            ZipFile.ExtractToDirectory(DownloadResult, target, true);
           
            foreach (String absFile in Directory.GetFiles(target))
            { 
                String relFile = Path.GetFileName(absFile);
                File.Move(Path.Combine(target, relFile), Path.Combine(Win64Dir, relFile), true);
            }

            Directory.Delete(target);
        }
    }
}
