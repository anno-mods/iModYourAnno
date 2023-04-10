using Imya.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.ModTweaker.DataModel.Storage
{
    public class TweakerFileStorageModel
    {
        /// <summary>
        /// Expose ID <-> Stored Value
        /// </summary>
        public Dictionary<string, string> Values { get; set; } = new();

        public string? GetTweakValue(string ExposeID)
        {
            return Values.SafeGet(ExposeID);
        }

        public bool HasStoredValue(string ExposeID) => Values.ContainsKey(ExposeID);

        public void SetTweakValue(string ExposeID, string Value) => Values[ExposeID] = Value;
    }
}
