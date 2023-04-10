using Imya.Services;
using Imya.Services.Interfaces;
using Imya.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.ModTweaker.DataModel.Storage
{
    /// <summary>
    /// A quick and dirty way to store the tweaks we did. 
    /// </summary>
    public class TweakRepository : ITweakRepository
    {
        private readonly IImyaSetupService _imyaSetupService;

        public TweakRepository(IImyaSetupService imyaSetupService)
        {
            _imyaSetupService = imyaSetupService;
        }

        private Dictionary<string, ModTweaksStorageModel> _storedTweaksById = new();

        /// <summary>
        /// Gets the Tweak Storage for an ID. If it does not exist, it creates a new one.
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ModTweaksStorageModel Get(string ID)
        {
            return _storedTweaksById.SafeAddOrGet(ID);
        }

        public bool IsStored(string ID)
        {
            return File.Exists(Path.Combine(_imyaSetupService.TweakDirectoryPath, ID + ".json"));
        }

        public IEnumerable<ModTweaksStorageModel> GetAllStorages()
        {
            return _storedTweaksById.Values.ToList();
        }

        public void SaveAll()
        {
            foreach (var (key, value) in _storedTweaksById)
            {
                value.Save(key);
            }
        }
    }

}
