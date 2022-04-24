using Imya.Models.Installation;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Utils
{
    public static class CollectionExtension
    {
        public static IEnumerable<TResult> SelectNoNull<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult?> selector) where TResult : class
        {
            return source.Select(selector).Where(x => x is not null).Select(x => x!);
        }
    }

    public static class DirectoryEx
    {
        /// <summary>
        /// Delete target folder before moving.
        /// </summary>
        public static void CleanMove(string source, string target)
        {
            if (Directory.Exists(target))
                Directory.Delete(target, true);
            Directory.Move(source, target);
        }

        /// <summary>
        /// Recursively copy a folder.
        /// </summary>
        public static void Copy(string source, string target)
        {
            if (!Directory.Exists(target))
                Directory.CreateDirectory(target);
            foreach (string file in Directory.EnumerateFiles(source))
                File.Copy(file, Path.Combine(target, Path.GetFileName(file)));
            foreach (string folder in Directory.EnumerateDirectories(source))
                Copy(folder, Path.Combine(target, Path.GetFileName(folder)));
        }

        /// <summary>
        /// Delete target folder before recursively copying source into it.
        /// </summary>
        public static void CleanCopy(string source, string target)
        {
            if (Directory.Exists(target))
                Directory.Delete(target, true);
            Copy(source, target);
        }

        /// <summary>
        /// Delete folder if it exists.
        /// </summary>
        /// <param name="path"></param>
        public static void EnsureDeleted(string path)
        {
            if (Directory.Exists(path))
                Directory.Delete(path, true);
        }
    }

    public static class VersionEx
    {
        /// <summary>
        /// Append ".0" in case of one number versions before parsing it.
        /// </summary>
        public static bool TryParse(string? input, out Version? result)
        {
            if (input is not null && !input.Contains('.'))
                input += ".0";
            return Version.TryParse(input, out result);
        }
    }

    //https://stackoverflow.com/questions/43661211/extract-an-archive-with-progress-bar

    public static class ZipArchiveExtensions
    {
        public static void ExtractToDirectory(this ZipArchive source, string destinationDirectoryName, IProgress<float>? progress = null, bool overwrite = false)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (destinationDirectoryName == null)
                throw new ArgumentNullException(nameof(destinationDirectoryName));


            // Rely on Directory.CreateDirectory for validation of destinationDirectoryName.

            // Note that this will give us a good DirectoryInfo even if destinationDirectoryName exists:
            DirectoryInfo di = Directory.CreateDirectory(destinationDirectoryName);
            string destinationDirectoryFullPath = di.FullName;

            int count = 0;
            foreach (ZipArchiveEntry entry in source.Entries)
            {
                count++;
                string fileDestinationPath = Path.GetFullPath(Path.Combine(destinationDirectoryFullPath, entry.FullName));

                if (!fileDestinationPath.StartsWith(destinationDirectoryFullPath, StringComparison.OrdinalIgnoreCase))
                    throw new IOException("File is extracting to outside of the folder specified.");

                progress?.Report( ((float)count / source.Entries.Count) * 0.9f);

                if (Path.GetFileName(fileDestinationPath).Length == 0)
                {
                    // If it is a directory:

                    if (entry.Length != 0)
                        throw new IOException("Directory entry with data.");

                    Directory.CreateDirectory(fileDestinationPath);
                }
                else
                {
                    // If it is a file:
                    // Create containing directory:
                    Directory.CreateDirectory(Path.GetDirectoryName(fileDestinationPath));
                    entry.ExtractToFile(fileDestinationPath, overwrite: overwrite);
                }
            }
        }
    }
}