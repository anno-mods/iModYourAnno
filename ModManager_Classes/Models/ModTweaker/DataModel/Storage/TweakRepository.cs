using Imya.Models.ModTweaker.IO;
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
        private readonly ModTweaksStorageModelLoader _storageModelLoader;

        public TweakRepository(
            IImyaSetupService imyaSetupService,
            ModTweaksStorageModelLoader storageModelLoader)
        {
            _imyaSetupService = imyaSetupService;
            _storageModelLoader = storageModelLoader;
        }

        private Dictionary<string, ModTweaksStorageModel> loadedStorages = new();

        /// <summary>
        /// Gets the Tweak Storage for an ID. If it does not exist, it creates a new one.
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ModTweaksStorageModel Get(string ID)
        {
            var filename = GetFilepath(ID);
            if (!File.Exists(filename))
                return new ModTweaksStorageModel();
            return _storageModelLoader.Load(GetFilepath(ID)) ?? new ModTweaksStorageModel();
        }

        public bool IsStored(string ID)
        {
            return File.Exists(GetFilepath(ID));
        }

        public void UpdateStorage(ModTweaksStorageModel storageModel, string modBaseName)
        {
            _storageModelLoader.Save(storageModel, GetFilepath(modBaseName));
        }

        private String GetFilepath(String baseName) => Path.Combine(_imyaSetupService.TweakDirectoryPath, baseName + ".json");
    }

}
