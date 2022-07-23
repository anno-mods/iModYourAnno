using Imya.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.ModTweaker
{
    /// <summary>
    /// A quick and dirty way to store the tweaks we did. 
    /// </summary>
    public class TweakStorageShelf
    {
        public static TweakStorageShelf Global { get; } = new TweakStorageShelf();

        public TweakStorageShelf() { }

        private Dictionary<String, TweakFileStorage> Tweaks = new(); 

        /// <summary>
        /// Gets the Tweak Storage for an ID. If it does not exist, it creates a new one.
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ITweakStorage Get(String ID)
        {
            return Tweaks.SafeAddOrGet(ID);
        }

        public bool IsStored(String ID)
        {
            return File.Exists(Path.Combine(ImyaSetupManager.Instance.TweakDirectoryPath, ID + ".json"));
        }

        public IEnumerable<ITweakStorage> GetAllStorages()
        {
            return Tweaks.Values.ToList();
        }

        public void SaveAll()
        {
            foreach (var (key, value) in Tweaks)
            {
                value.Save(key);
            }
        }
    }

    public class Tweak
    {
        /// <summary>
        /// Expose ID <-> Stored Value
        /// </summary>
        public Dictionary<String, String> Values { get; set; } = new();

        public String? GetTweakValue(String ExposeID)
        {
            return Values.SafeGet(ExposeID);
        }
        public void SetTweakValue(String ExposeID, String Value) => Values[ExposeID] = Value;
    }
}
