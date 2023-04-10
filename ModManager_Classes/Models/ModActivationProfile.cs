using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imya.Models.Mods;

namespace Imya.Models
{
    public class ModActivationProfile : IEnumerable<string>
    {
        public static string ProfileExtension { get; } = "imyaprofile";

        private List<string> ModFolderNames = new();
        public string Title { get; set; } = "DummyTitle";

        /// <summary>
        /// Filename the profile was loaded from. This is only set if the profile was loaded from a file.
        /// </summary>
        public string? Filename { get; set; }
        public bool HasFilename => Filename is not null;

        public static ModActivationProfile FromModCollection(ModCollection collection, Func<Mod, bool> SelectFunction)
        {
            ModActivationProfile profile = new();
            profile.ModFolderNames = collection.Mods
                .Where(SelectFunction)
                .Select(x => x.FolderName)
                .ToList();
            return profile;
        }

        public static ModActivationProfile? FromFile(string _filename)
        {
            ModActivationProfile profile = new();
            try
            {
                using StreamReader reader = new(File.OpenRead(_filename));
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    //validation prettyplease?

                    if (line is not null)
                        profile.ModFolderNames.Add(line);
                }

                profile.Title = Path.GetFileNameWithoutExtension(_filename);
                profile.Filename = _filename;
                return profile;
            }
            catch (IOException)
            {
                Console.WriteLine($"Could not access File: {_filename}");
                return null;
            }
        }

        public bool SaveToFile(string _filename)
        {
            try 
            {
                FileStream fs = File.Create(_filename);
                SaveToStream(fs);
                return true;
            }
            catch (IOException)
            {
                Console.WriteLine($"Could not create File: {_filename}");
                return false;
            }
        }

        public void SaveToStream(Stream s)
        {
            using StreamWriter writer = new(s);
            foreach (string dir_name in ModFolderNames)
            {
                writer.WriteLine(dir_name);
                writer.Flush();
            }
        }

        public bool IsEmpty()
        {
            return !ModFolderNames.Any();
        }

        public IEnumerator<string> GetEnumerator()
        {
            return ModFolderNames.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ModFolderNames.GetEnumerator();
        }
    }
}
