
using Imya.Models.ModTweaker;
using Imya.Models.Installation;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

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
        public static void EnsureDeleted(string path)
        {
            if (Directory.Exists(path))
                Directory.Delete(path, true);
        }

        /// <summary>
        /// Delete folder if it exists. No Exceptions
        /// </summary>
        public static bool TryDelete(string path)
        {
            try
            {
                if (Directory.Exists(path))
                    Directory.Delete(path, true);
            }
            catch
            {
                return false;
            }
            return true;
        }        

        /// <summary>
        /// Find paths with a folder name.
        /// </summary>
        public static IEnumerable<string> FindFolder(string path, string folderName)
        {
            List<string> result = new();
            Queue<string> queue = new(Directory.EnumerateDirectories(path));
            while (queue.Count > 0)
            {
                string folder = queue.Dequeue();
                if (Path.GetFileName(folder) == folderName)
                {
                    result.Add(folder);
                }
                else
                {
                    foreach (var add in Directory.EnumerateDirectories(folder))
                        queue.Enqueue(add);
                }
            }
            return result;
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

    public static class DictionaryEx
    {
        public static TValue? SafeGet<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey Key) where TKey : notnull
        {
            try
            {
                return dict[Key];
            }
            catch (Exception e)
            {
                return default(TValue);
            }
        }

        public static TValue SafeAddOrGet<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey Key) where TKey : notnull where TValue : new()
        {
            if (dict.ContainsKey(Key)) return dict.SafeGet(Key)!;

            TValue t = new();
            dict.Add(Key, t);
            return t;
        }
    }

    public static class XmlNodeExtensions
    {
        public static bool HasAttribute(this XmlNode node, String AttribID)
        {
            return node.Attributes?[AttribID] is not null;
        }

        public static bool TryGetAttribute(this XmlNode node, String AttribID, out String? Value)
        {
            Value = node.Attributes?[AttribID]?.Value;
            return Value is not null;
        }

        public static bool TryGetModOpNode(this XmlDocument Document, String ModOpId, out XmlNode? ModOp)
        {
            ModOp = Document.SelectSingleNode($@"/ModOps/ModOp[@{TweakerConstants.MODOP_ID} = '{ModOpId}']");
            return ModOp is not null;
        }

        public static bool TryGetModOpNodes(this XmlDocument Document, String ModOpId, out XmlNodeList? ModOps)
        {
            ModOps = Document.SelectNodes($@"/ModOps/*[(name()='ModOp' or name()='Include') and @{TweakerConstants.MODOP_ID} = '{ModOpId}']");
            return ModOps is not null && ModOps.Count > 0;
        }
    }

    //https://stackoverflow.com/questions/4238345/asynchronously-wait-for-taskt-to-complete-with-timeout
    public static class TaskExtensions
    {
        public static async Task<TResult> TimeoutAfter<TResult>(this Task<TResult> task, TimeSpan timeout)
        {
            using (var timeoutCancellationTokenSource = new CancellationTokenSource())
            {

                var completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
                if (completedTask == task)
                {
                    timeoutCancellationTokenSource.Cancel();
                    return await task;  // Very important in order to propagate exceptions
                }
                else
                {
                    throw new TimeoutException("The operation has timed out.");
                }
            }
        }

        public static async Task TimeoutAfter(this Task task, TimeSpan timeout)
        {
            using (var timeoutCancellationTokenSource = new CancellationTokenSource())
            {
                var completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
                if (completedTask == task)
                {
                    timeoutCancellationTokenSource.Cancel();
                    return;  // Very important in order to propagate exceptions
                }
                else
                {
                    throw new TimeoutException("The operation has timed out.");
                }
            }
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
