using System;
using System.Collections.Generic;
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
        public static void CleanMove(string sourceDirName, string destDirName)
        {
            if (Directory.Exists(destDirName))
                Directory.Delete(destDirName, true);
            Directory.Move(sourceDirName, destDirName);
        }
    }
}
