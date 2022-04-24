using Imya.Models.ModTweaker;
using System;
using System.Collections.Generic;
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
            ModOps = Document.SelectNodes($@"/ModOps/ModOp[@{TweakerConstants.MODOP_ID} = '{ModOpId}']");
            return ModOps is not null && ModOps.Count > 0;
        }
    }
}
