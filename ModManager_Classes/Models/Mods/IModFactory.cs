using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.Mods
{
    public interface IModFactory
    {
        Mod? GetFromFolder(string modFolderPath, bool loadImages = false);
    }
}
