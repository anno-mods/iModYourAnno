using Imya.Models;
using Imya.Models.Attributes.Factories;
using Imya.Models.Mods;
using Imya.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Services
{
    public class ProfilesService : IProfilesService
    {
        private static readonly string _profileFileExtension = ".imyaprofile";

        private readonly IImyaSetupService _imyaSetupService;

        public ProfilesService(IImyaSetupService imyaSetupService)
        {
            _imyaSetupService = imyaSetupService;
        }

        private IEnumerable<ModActivationProfile> GetSavedProfiles()
        {
            var files = Directory.EnumerateFiles(_imyaSetupService.ProfilesDirectoryPath, "*" + _profileFileExtension);
            return files?.Select(file => LoadProfile(file))?.Where(x => x is not null)?.ToArray() ?? Enumerable.Empty<ModActivationProfile>();
        }

        public IEnumerable<String> GetSavedKeys()
        {
            var filepaths = Directory.EnumerateFiles(_imyaSetupService.ProfilesDirectoryPath, "*" + _profileFileExtension);
            return filepaths.Select(x => Path.GetFileNameWithoutExtension(x)).ToArray();
        }

        public ModActivationProfile CreateFromModCollection(ModCollection collection)
        {
            var foldernames = collection.Mods
                .Where(x => x.IsActive)
                .Select(x => x.FolderName)
                .ToList();
            return new ModActivationProfile(foldernames);
        }

        public void DeleteActivationProfile(String key)
        {
            var filepath = GetFilepath(key);
            if(File.Exists(filepath))
                File.Delete(GetFilepath(key));
        }

        public void SaveProfile(ModActivationProfile profile, String key)
        {
            string filepath = GetFilepath(key);
            try
            {
                using StreamWriter writer = new(File.Create(filepath));
                foreach (string dir_name in profile.ModFolderNames)
                {
                    writer.WriteLine(dir_name);
                    writer.Flush();
                }
            }
            catch (IOException e)
            {
                Console.WriteLine($"Could not create File: {filepath}");
                return;
            }
        }

        public bool ProfileExists(String key)
        {
            return File.Exists(GetFilepath(key));
        }

        public ModActivationProfile? LoadProfile(String key)
        {
            List<string> parsedNames = new();
            var filename = GetFilepath(key);

            try
            {
                using StreamReader reader = new(File.OpenRead(filename));
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    if (line is not null)
                        parsedNames.Add(line);
                }
                return new ModActivationProfile(parsedNames)
                {
                    Title = key
                };
            }
            catch (IOException)
            {
                Console.WriteLine($"Could not access File: {filename}");
                return null;
            }
        }

        private String GetFilepath(String key) => Path.Combine(_imyaSetupService.ProfilesDirectoryPath, key + _profileFileExtension);
    }
}
