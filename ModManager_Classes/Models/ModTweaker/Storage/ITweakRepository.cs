using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.ModTweaker.Storage
{
    public interface ITweakRepository
    {
        ITweakStorage Get(String ID);
        IEnumerable<ITweakStorage> GetAllStorages();

        bool IsStored(String ID);
        void SaveAll();
    }
}
