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
        private List<String> DirectoryNames;

        public String Title { get; set; } = "DummyTitle";

        public ModActivationProfile()
        {
            DirectoryNames = new List<String>();
        }

        public void LoadFromFile(String Filename)
        {
            try
            {
                FileStream fs = File.OpenRead(Filename);
                LoadFromStream(fs);
            }
            catch (IOException e)
            {
                Console.WriteLine($"Could not access File: {Filename}");
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
