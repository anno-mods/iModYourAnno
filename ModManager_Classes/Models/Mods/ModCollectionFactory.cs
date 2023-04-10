using System;
using System.IO.Compression;
using Imya.Models.Installation;
using Imya.Services.Interfaces;
using Imya.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace Imya.Models.Mods
{
    /// <summary>
    /// Install mods from zip file - might depracate this honestly
    /// </summary>
    public class ModCollectionFactory : IModCollectionFactory
    {
        private readonly IServiceProvider _serviceProvider;
        public ModCollectionFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ModCollection Get(
            string Filepath,
            bool normalize = false,
            bool loadImages = false,
            bool autofixSubfolder = false)
        {
            var collection = new ModCollection(
                _serviceProvider.GetRequiredService<IGameSetupService>(),
                _serviceProvider.GetRequiredService<ModCollectionHooks>())
            {
                ModsPath = Filepath,
                Normalize = normalize,
                LoadImages = loadImages,
                AutofixSubfolder = autofixSubfolder
            };
            return collection;
        }
    }
}
