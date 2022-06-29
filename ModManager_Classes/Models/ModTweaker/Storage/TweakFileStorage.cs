using Imya.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.ModTweaker
{

    public class TweakFileStorage : ITweakStorage
    {
        public Dictionary<String, Tweak> Tweaks { get; set; } = new();

        public static String BaseDirectory => ImyaSetupManager.Instance.TweakDirectoryPath;

        public TweakFileStorage()
        {
            if(!Directory.Exists(BaseDirectory))
            Directory.CreateDirectory(BaseDirectory);
        }

        public void SetTweakValue(string Filename, string ExposeID, string NewValue)
        {
            AddOrGetTweak(Filename).SetTweakValue(ExposeID, NewValue);
        }

        public bool TryGetTweakValue(string Filename, string ExposeID, out string? Value)
        {
            Value = GetTweak(Filename)?.GetTweakValue(ExposeID);
            return Value is not null;
        }

        public Tweak? GetTweak(String Filename)
        {
            return Tweaks.SafeGet(Filename);
        }

        public Tweak AddOrGetTweak(String Filename)
        {
            return Tweaks.SafeAddOrGet(Filename);
        }

        public void Save(String FilenameBase)
        {
            //filter name to be on the safe side.
            FilenameBase = Path.GetFileName(FilenameBase);

            String s = JsonConvert.SerializeObject(Tweaks, Formatting.Indented);
            using (StreamWriter writer = new StreamWriter(File.Create($"{Path.Combine(BaseDirectory, FilenameBase)}.json")))
            {
                writer.Write(s);
            }
        }

        public static TweakFileStorage? LoadFromFile(String FilenameBase)
        {
            try
            {
                return LoadFromJson(File.ReadAllText($"{Path.Combine(BaseDirectory, FilenameBase)}.json"));
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Loads a Tweak Collection From the file with the specified ID. If there is no stored collection, it creates a new one.
        /// </summary>
        /// <param name="FilenameBase"></param>
        /// <returns>The tweak collection</returns>
        public static TweakFileStorage LoadOrCreate(String FilenameBase)
        {
            var coll = LoadFromFile(FilenameBase);
            if (coll is not null) return coll;

            return new TweakFileStorage();
        }

        public static TweakFileStorage? LoadFromJson(String JsonString)
        {
            try
            {
                var coll = new TweakFileStorage();
                var tweaks = JsonConvert.DeserializeObject<Dictionary<String, Tweak>>(JsonString);
                if (tweaks is null) return null;
                coll.Tweaks = tweaks;

                return coll;
            }
            catch (JsonSerializationException e)
            {
                Console.WriteLine("Could not load Tweak Collection");
                return null;
            }
        }
    }
}