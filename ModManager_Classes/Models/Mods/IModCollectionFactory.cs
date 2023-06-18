using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.Mods
{
    public interface IModCollectionFactory
    {
        ModCollection Get(string Filepath,
            bool normalize = false,
            bool loadImages = false,
            bool autofixSubfolder = false);
    }
}
