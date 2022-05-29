using System.Security.Cryptography;
using System.Text;

namespace Imya.Utils
{
    internal class FilePathComparer : IEqualityComparer<FileInfo>
    {
        private string _prefixPathA;
        private string _prefixPathB;

        public FilePathComparer(string prefixPathA, string prefixPathB)
        {
            _prefixPathA = prefixPathA;
            _prefixPathB = prefixPathB;
        }

        public bool Equals(FileInfo? a, FileInfo? b)
        {
            if (a is null && b is null)
                return true;
            if (a is null || b is null)
                return false;

            if (Path.GetRelativePath(_prefixPathA, a.FullName) != Path.GetRelativePath(_prefixPathB, b.FullName))
                return false;

            // check length also because we have the info already
            return a.Length == b.Length;
        }

        public int GetHashCode(FileInfo obj)
        {
            return obj.GetHashCode();
        }
    }

    /// <summary>
    /// Note: Avoid GetHashCode. They recalculate MD5 of the file every time.
    /// </summary>
    internal class FileMd5Comparer : IEqualityComparer<FileInfo>
    {
        public bool Equals(FileInfo? a, FileInfo? b)
        {
            if (a is null && b is null)
                return true;
            if (a is null || b is null)
                return false;

            // obvious differences
            if (a.Length != b.Length)
                return false;

            // MD5 difference
            return GetMd5(a) == GetMd5(b);
        }

        public int GetHashCode(FileInfo obj)
        {
            var hash = new HashCode();
            hash.Add(obj);
            hash.Add(GetMd5(obj));
            return hash.ToHashCode();
        }

        private static string GetMd5(FileInfo fileInfo)
        {
            using var md5 = MD5.Create();
            using var stream = File.OpenRead(fileInfo.FullName);
            return Encoding.Default.GetString(md5.ComputeHash(stream));
        }
    }
}
