using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models
{
    public class ModActivationProfile
    {
        private IEnumerable<String> ModIDs;

        public ModActivationProfile()
        {

        }

        public bool ContainsID(String id)
        {
            return ModIDs.Any( x => x.Equals(id));
        }

        public void LoadFromStream(Stream s)
        {
            ModIDs = ParseStream(s);
        }

        private IEnumerable<String> ParseStream(Stream s)
        {
            s.Position = 0;
            using (StreamReader reader = new StreamReader(s))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if(line is String) yield return line;
                }
            }
        }
    }
}
