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
        bool IsStored(string ID);
        void UpdateStorage(ModTweaksStorageModel storageModel, string modBaseName);
    }
}
