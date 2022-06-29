using System.IO.Compression;
using Imya.Models;
using Imya.Models.Installation;

namespace Imya.Utils
{
    /// <summary>
    /// Install mods from zip file - might depracate this honestly
    /// </summary>
    public class ModInstaller
    {
        public static async Task<ModCollection?> ExtractZipAsync(string zipFilePath, string tempDir, Installation? progress = null)
        {
            progress?.SetProgressRange(0, 0.9f);
            progress?.Report(0);

            // TODO issue handling
            if (!Directory.Exists(tempDir) || !File.Exists(zipFilePath)) return null;

            string extractTarget = Path.Combine(tempDir, Path.GetFileNameWithoutExtension(zipFilePath));
            
            if (Directory.Exists(extractTarget))
                Directory.Delete(extractTarget, true);

            using (FileStream fs = File.OpenRead(zipFilePath))
            using (ZipArchive archive = new ZipArchive(fs, ZipArchiveMode.Read))
            {
                archive.ExtractToDirectory(extractTarget, progress, overwrite: true);
            }
            progress?.Report(1f);

            progress?.SetProgressRange(0.9f, 1);

            var collection = new ModCollection(extractTarget);
            await collection.LoadModsAsync();

            progress?.Report(1f);
            return collection;
        }
    }
}
