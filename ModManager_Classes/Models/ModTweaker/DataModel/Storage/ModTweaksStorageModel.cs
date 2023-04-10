using Imya.Services;
using Imya.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.ModTweaker.DataModel.Storage
{
    public class ModTweaksStorageModel : IModTweaksStorageModel
    {
        public Dictionary<string, TweakerFileStorageModel> Tweaks { get; set; } = new();

        public static string BaseDirectory => ImyaSetupService.Instance.TweakDirectoryPath;

        public ModTweaksStorageModel()
        {
            if (!Directory.Exists(BaseDirectory))
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

        public TweakerFileStorageModel? GetTweak(string Filename)
        {
            return Tweaks.SafeGet(Filename);
        }

        public TweakerFileStorageModel AddOrGetTweak(string Filename)
        {
            return Tweaks.SafeAddOrGet(Filename);
        }

        public void Save(string FilenameBase)
        {
            //filter name to be on the safe side.
            FilenameBase = Path.GetFileName(FilenameBase);

            string s = JsonConvert.SerializeObject(Tweaks, Formatting.Indented);
            using (StreamWriter writer = new StreamWriter(File.Create($"{Path.Combine(BaseDirectory, FilenameBase)}.json")))
            {
                writer.Write(s);
            }
        }

        public static ModTweaksStorageModel? LoadFromFile(string FilenameBase)
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
        public static ModTweaksStorageModel LoadOrCreate(string FilenameBase)
        {
            var coll = LoadFromFile(FilenameBase);
            if (coll is not null) return coll;

            return new ModTweaksStorageModel();
        }

        public static ModTweaksStorageModel? LoadFromJson(string JsonString)
        {
            try
            {
                var coll = new ModTweaksStorageModel();
                var tweaks = JsonConvert.DeserializeObject<Dictionary<string, TweakerFileStorageModel>>(JsonString);
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