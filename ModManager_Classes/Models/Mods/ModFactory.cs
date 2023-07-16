using Imya.Models.Attributes;
using Imya.Models.Attributes.Factories;
using Imya.Models.Attributes.Interfaces;
using Imya.Models.ModMetadata;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.Mods
{
    public class ModFactory : IModFactory
    {
        private readonly IMissingModinfoAttributeFactory _missingModinfoAttributeFactory;
        private readonly LocalizedModinfoFactory _localizedModinfoFactory;
        
        public ModFactory(
            IMissingModinfoAttributeFactory missingModinfoAttributeFactory,
            LocalizedModinfoFactory localizedModinfoFactory)
        {
            _missingModinfoAttributeFactory = missingModinfoAttributeFactory;
            _localizedModinfoFactory = localizedModinfoFactory;
        }

        public Mod? GetFromFolder(string modFolderPath, bool loadImages = false)
        { 
            var basePath = Path.GetDirectoryName(modFolderPath); 
            if (basePath is null || !Directory.Exists(modFolderPath)) 
                return null;

            var folder = Path.GetFileName(modFolderPath);
            var isActive = !folder.StartsWith("-");
            var folderName = isActive ? folder : folder[1..];

            bool hasModinfo = ModinfoLoader.TryLoadFromFile(Path.Combine(modFolderPath, "modinfo.json"), out var modinfo);

            var localizedModinfo = hasModinfo ?
                _localizedModinfoFactory.GetLocalizedModinfo(modinfo!, folderName)
                : _localizedModinfoFactory.GetDummyModinfo(folderName);

            var mod = new Mod(
                isActive,
                folderName,
                localizedModinfo,
                basePath);

            if (!hasModinfo)
                mod.Attributes.AddAttribute(_missingModinfoAttributeFactory.Get());

            string[] modinfos = Directory.GetFiles(Path.Combine(basePath, folder), "modinfo.json", SearchOption.AllDirectories);

            if (loadImages)
            {
                var imagepath = Path.Combine(mod.FullModPath, "banner.jpg");
                if (File.Exists(imagepath))
                    mod.InitImageAsFilepath(Path.Combine(imagepath));
                else
                {
                    imagepath = Path.Combine(mod.FullModPath, "banner.png");
                    if (File.Exists(imagepath))
                        mod.InitImageAsFilepath(Path.Combine(imagepath));
                }
            }

            if (modinfos.Length > 1)
            {
                foreach (var submodinfo in modinfos)
                {
                    if (submodinfo.ToLower() == Path.Combine(basePath, folder, "modinfo.json").ToLower())
                    {
                        continue;
                    }

                    Mod? submod = GetFromFolder(Path.GetDirectoryName(submodinfo) ?? "", loadImages: true);
                    if (submod is not null)
                    {
                        mod.SubMods.Add(submod);
                    }
                }
            }


            return mod; 
        }
    }
}
