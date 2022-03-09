using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.ModMetadata
{
    public class ModinfoLoader
    {
        public static bool TryLoadFromFile(String Filename, out Modinfo metadata)
        {
            try
            {
                metadata = JsonConvert.DeserializeObject<Modinfo>(File.ReadAllText(Filename));
                Console.WriteLine($"Loaded Modinfo file from {Filename}");
                return true;
            }
            catch (JsonSerializationException e)
            {
                metadata = null;
                Console.WriteLine("Json Serialization failed: {0}", Filename);
            }
            catch (IOException e)
            {
                metadata = null;
                Console.WriteLine("File not found: {0}", Filename);
            }
            return false;
        }

        public static bool TrySaveToFile(String Filename, Modinfo m)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(File.Create(Filename)))
                {
                    writer.Write(JsonConvert.SerializeObject(m, Formatting.Indented));
                }
                Console.WriteLine($"Saved Modinfo file to {Filename}");
                return true;
            }
            catch (JsonSerializationException e)
            {
                Console.WriteLine("Json Serialization failed: {0}", Filename);
            }
            catch (IOException e)
            {
                Console.WriteLine("File not found: {0}", Filename);
            }
            return false;
        }
    }
}
