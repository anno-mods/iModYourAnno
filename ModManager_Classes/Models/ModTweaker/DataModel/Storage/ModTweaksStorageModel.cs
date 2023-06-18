using Imya.Services;
using Imya.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.ModTweaker.DataModel.Storage
{
    public class ModTweaksStorageModel : IModTweaksStorageModel
    {
        public Dictionary<string, TweakerFileStorageModel> Tweaks { get; set; } = new();

        public ModTweaksStorageModel()
        {

        }

        public void SetTweakValue(string Filename, string ExposeID, string NewValue)
        {
            AddOrGetTweak(Filename).SetTweakValue(ExposeID, NewValue);
        }

        public bool TryGetTweakValue(string Filename, string ExposeID, out string? Value)
        {
            Value = GetTweak(Filename)?.GetTweakValue(ExposeID);
            return Value is not null;
        }

        public TweakerFileStorageModel? GetTweak(string Filename)
        {
            return Tweaks.SafeGet(Filename);
        }

        public TweakerFileStorageModel AddOrGetTweak(string Filename)
        {
            return Tweaks.SafeAddOrGet(Filename);
        }
    }
}