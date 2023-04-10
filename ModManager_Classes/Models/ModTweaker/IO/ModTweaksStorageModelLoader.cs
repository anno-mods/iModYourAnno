using Imya.Models.ModTweaker.DataModel.Storage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.ModTweaker.IO
{
    public class ModTweaksStorageModelLoader
    {
        public void Save(ModTweaksStorageModel model, String filename)
        {
            string s = JsonConvert.SerializeObject(model.Tweaks, Formatting.Indented);
            using (StreamWriter writer = new StreamWriter(File.Create(filename)))
            {
                writer.Write(s);
            }
        }

        public ModTweaksStorageModel? Load(String filename)
        {
            var jsonString = File.ReadAllText(filename);
            try
            {
                var storageModel = new ModTweaksStorageModel();
                var tweaks = JsonConvert.DeserializeObject<Dictionary<string, TweakerFileStorageModel>>(jsonString);
                if (tweaks is null) return null;
                storageModel.Tweaks = tweaks;

                return storageModel;
            }
            catch (JsonSerializationException e)
            {
                Console.WriteLine("Could not load Tweak Collection");
                return null;
            }
        }
    }
}
