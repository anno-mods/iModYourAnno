using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models
{
    public class ModActivationProfile : IEnumerable<String>
    {
        public static String ProfileExtension { get; } = "imyaprofile";

        private List<String> DirectoryNames;
        public String Title { get; set; } = "DummyTitle";

        /// <summary>
        /// Filename the profile was loaded from. This is only set if the profile was loaded from a file.
        /// </summary>
        public String? Filename { get; set; }
        public bool HasFilename => Filename is not null;

        public ModActivationProfile()
        {
            DirectoryNames = new List<String>();
        }

        public static ModActivationProfile FromModCollection(ModCollection collection, Func<Mod, bool> SelectFunction)
        {
            ModActivationProfile profile = new ModActivationProfile();

            foreach (Mod m in collection.Mods)
            {
                if(SelectFunction.Invoke(m)) profile.DirectoryNames.Add(m.FolderName);
            }

            return profile;
        }

        public bool LoadFromFile(String _filename)
        {
            try
            {
                FileStream fs = File.OpenRead(_filename);
                LoadFromStream(fs);

                Title = Path.GetFileNameWithoutExtension(_filename);
                Filename = _filename;

                return true;
            }
            catch (IOException e)
            {
                Console.WriteLine($"Could not access File: {_filename}");
                return false;
            }
        }

        public bool SaveToFile(String _filename)
        {
            try 
            {
                FileStream fs = File.Create(_filename);
                SaveToStream(fs);
                return true;
            }
            catch (IOException e)
            {
                Console.WriteLine($"Could not create File: {_filename}");
                return false;
            }
        }

        public void LoadFromStream(Stream s)
        {
            using (StreamReader reader = new StreamReader(s))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    //validation prettyplease?

                    if(line is String) DirectoryNames.Add(line);
                }
            }
        }

        public void SaveToStream(Stream s)
        {
            using (StreamWriter writer = new StreamWriter(s))
            {
                foreach (String dir_name in DirectoryNames)
                {
                    writer.WriteLine(dir_name);
                    writer.Flush();
                }
            }
        }

        public bool IsEmpty()
        {
            return !DirectoryNames.Any();
        }

        public IEnumerator<string> GetEnumerator()
        {
            return DirectoryNames.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return DirectoryNames.GetEnumerator();
        }
    }
}
