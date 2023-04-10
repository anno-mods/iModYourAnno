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

        private Dictionary<string, ModTweaksStorageModel> loadedStorages = new();

        /// <summary>
        /// Gets the Tweak Storage for an ID. If it does not exist, it creates a new one.
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ModTweaksStorageModel Get(string ID)
        {
            var storagePath = GetFilepath(ID);
            if (File.Exists(storagePath))
            {
                var text = File.ReadAllText(storagePath); 
                try
                {
                    JsonConvert.DeserializeObject<ModTweaksStorageModel>(text);
                }
                catch (Exception ex) 
                {
                    Console.WriteLine("Could not load storage: " + storagePath);
                }
            }
            return new ModTweaksStorageModel();
        }

        public bool IsStored(string ID)
        {
            return File.Exists(GetFilepath(ID));
        }

        public void UpdateStorage(ModTweaksStorageModel storageModel, string modBaseName)
        {
            var storagePath = GetFilepath(modBaseName);
            var json = JsonConvert.SerializeObject(storageModel);
            File.WriteAllText(storagePath, json);
        }

        private String GetFilepath(String baseName) => Path.Combine(_imyaSetupService.TweakDirectoryPath, baseName + ".json");
    }

}
