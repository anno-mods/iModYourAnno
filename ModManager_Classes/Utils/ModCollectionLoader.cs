using System.IO.Compression;
using Imya.Models;
using Imya.Models.Installation;

namespace Imya.Utils
{
    /// <summary>
    /// Install mods from zip file - might depracate this honestly
    /// </summary>
    public class ModCollectionLoader
    {
        public static async Task<ModCollection?> LoadFrom(String Filepath)
        {
            var collection = new ModCollection(Filepath, autofixSubfolder: true);
            await collection.LoadModsAsync();
            return collection;
        }
    }
}
