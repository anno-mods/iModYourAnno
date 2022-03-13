using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imya.GithubIntegration;

namespace Imya.Utils
{
    /// <summary>
    /// Install mods from zip file.
    /// </summary>
    public class ModInstaller
    {
        public class ModDirectoryTodo
        {
            // this will be very much like ModDirectoryManager, hence transform the manager into a reusable class
            public string ExtractedBase { get; init; } = "";
            public string[] ModFolders { get; init; } = Array.Empty<string>();
        }

        public static async Task<ModDirectoryTodo?> PrepareInstallZipAsync(string zipFilePath, string tempDir)
        {
            // TODO issue handling
            if (!Directory.Exists(tempDir) || !File.Exists(zipFilePath)) return null;

            string extractTarget = Path.Combine(tempDir, Path.GetFileNameWithoutExtension(zipFilePath));
            
            if (Directory.Exists(extractTarget))
                Directory.Delete(extractTarget, true);

            await Task.Run(() => ZipFile.ExtractToDirectory(zipFilePath, extractTarget, true));

            var modFolders = Directory.GetDirectories(extractTarget).Select(x => Path.GetRelativePath(extractTarget, x)).ToArray();
            return new ModDirectoryTodo() { ExtractedBase = extractTarget, ModFolders = modFolders };
        }

        public static async Task FinalizeInstallAsync(ModDirectoryTodo modDirectory, string modFolderPath)
        {
            // TODO issue handling
            await Task.Run(() => {
                foreach (var folder in modDirectory.ModFolders)
                {
                    var targetMod = Path.Combine(modFolderPath, folder);
                    // TODO move with overwrite instead of mod deletion
                    Directory.Delete(targetMod, true);
                    Directory.Move(Path.Combine(modDirectory.ExtractedBase, folder), targetMod);
                }

                Directory.Delete(modDirectory.ExtractedBase, true);
            });
        }
    }
}
