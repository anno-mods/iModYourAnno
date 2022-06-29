using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.ModTweaker
{
    public interface ITweakStorage
    {
        public void SetTweakValue(String Filename, String ExposeID, String NewValue);
        public bool TryGetTweakValue(String Filename, String ExposeID, out String? Value);

        public void Save(String FilenameBase);
    }
}
