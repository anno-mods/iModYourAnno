using System.IO.Compression;
using Imya.Models;

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

        public static async Task<ModCollection?> ExtractZipAsync(string zipFilePath, string tempDir)
        {
            // TODO issue handling
            if (!Directory.Exists(tempDir) || !File.Exists(zipFilePath)) return null;

            string extractTarget = Path.Combine(tempDir, Path.GetFileNameWithoutExtension(zipFilePath));
            
            if (Directory.Exists(extractTarget))
                Directory.Delete(extractTarget, true);

            ZipFile.ExtractToDirectory(zipFilePath, extractTarget, true);

            var collection = new ModCollection(extractTarget);
            await collection.LoadModsAsync();
            return collection;
        }
    }
}
