using Imya.Models;
using Imya.Models.Mods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Services.Interfaces
{
    public interface IProfilesService
    {
        IEnumerable<String> GetSavedKeys();
        ModActivationProfile CreateFromModCollection(ModCollection collection);
        ModActivationProfile? LoadProfile(String key);
        void DeleteActivationProfile(String key);
        void SaveProfile(ModActivationProfile profile, String key);
        bool ProfileExists(String key);
    }
}
