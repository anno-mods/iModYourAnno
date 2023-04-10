using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.ModTweaker.DataModel.Storage
{
    public interface ITweakRepository
    {
        ModTweaksStorageModel Get(string ID);
        IEnumerable<ModTweaksStorageModel> GetAllStorages();

        bool IsStored(string ID);
        void SaveAll();
    }
}
