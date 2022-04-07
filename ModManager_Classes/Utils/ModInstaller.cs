using System.IO.Compression;
using Imya.Models;

namespace Imya.Utils
{
    /// <summary>
    /// Install mods from zip file.
    /// </summary>
    public class ModInstaller
    {
        public static async Task<ModCollection?> ExtractZipAsync(string zipFilePath, string tempDir, IProgress<float>? progress = null)
        {
            progress?.Report(0);

            // TODO issue handling
            if (!Directory.Exists(tempDir) || !File.Exists(zipFilePath)) return null;

            string extractTarget = Path.Combine(tempDir, Path.GetFileNameWithoutExtension(zipFilePath));
            
            if (Directory.Exists(extractTarget))
                Directory.Delete(extractTarget, true);

            // TODO ZipFile doesn't have progress
            ZipFile.ExtractToDirectory(zipFilePath, extractTarget, true);
            progress?.Report(0.1f);

            var collection = new ModCollection(extractTarget);
            await collection.LoadModsAsync();

            progress?.Report(0.5f);
            return collection;
        }
    }
}
