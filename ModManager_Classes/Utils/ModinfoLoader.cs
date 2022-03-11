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
        public static bool TryLoadFromFile(string filePath, out Modinfo? metadata)
        {
            var shortPath = Path.Combine(Path.GetFileName(Path.GetDirectoryName(filePath)??""), Path.GetFileName(filePath));

            try
            {
                var settings = new JsonSerializerSettings();
                // Every field is optional, thus be kind and forgive errors.
                settings.Error += (obj, args) => {
                    args.ErrorContext.Handled = true;
                    Console.WriteLine($"Warning: {args.ErrorContext.Path} in {shortPath} is invalid");
                };
                metadata = JsonConvert.DeserializeObject<Modinfo>(File.ReadAllText(filePath), settings)??new Modinfo();
                Console.WriteLine($"Loaded Modinfo file from {shortPath}");
                return true;
            }
            catch (JsonSerializationException e)
            {
                metadata = null;
                Console.WriteLine("Json Serialization failed: {0}", shortPath);
            }
            catch (IOException e)
            {
                metadata = null;
                Console.WriteLine("File not found: {0}", shortPath);
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
