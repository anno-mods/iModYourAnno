using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models
{
    public class ModActivationProfile : IEnumerable<string>
    {
        public static string ProfileExtension { get; } = "imyaprofile";

        public IEnumerable<string> ModFolderNames = new List<String>();
        public string Title { get; set; } = "DummyTitle";

        private ModActivationProfile() {
        
        }

        public ModActivationProfile(IEnumerable<String> folderNames)
        {
            ModFolderNames = folderNames;
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
