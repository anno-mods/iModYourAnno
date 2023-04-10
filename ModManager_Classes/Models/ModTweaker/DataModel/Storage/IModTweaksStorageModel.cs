using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.ModTweaker.DataModel.Storage
{
    public interface IModTweaksStorageModel
    {
        public void SetTweakValue(string Filename, string ExposeID, string NewValue);
        public bool TryGetTweakValue(string Filename, string ExposeID, out string? Value);

        public void Save(string FilenameBase);

        public TweakerFileStorageModel GetTweak(string Filename);
    }
}
