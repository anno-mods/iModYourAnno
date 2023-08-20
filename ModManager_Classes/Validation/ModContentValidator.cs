using Anno.EasyMod.Mods;
using Imya.Models.Attributes;
using Imya.Models.Attributes.Interfaces;
using Imya.Utils;
using System.Collections.Specialized;

namespace Imya.Validation
{
    /// <summary>
    /// Checks if data/ is located in the first subfolder.
    /// That's a common mistake people make.
    /// </summary>
    public class ModContentValidator : IModValidator
    {
        private readonly IContentInSubfolderAttributeFactory _factory;
        public ModContentValidator(IContentInSubfolderAttributeFactory factory) 
        {
            _factory = factory;
        }

        public void Validate(IEnumerable<IMod> changed, IReadOnlyCollection<IMod> all, NotifyCollectionChangedAction changedAction)
        {
            foreach (var mod in changed)
                ValidateSingle(mod);
        }

        private void ValidateSingle(IMod mod)
        {
            if (mod.IsRemoved || !Directory.Exists(mod.FullModPath))
            {
                return;
            }

            string dataPath = Path.Combine(mod.FullModPath, "data");
            if (Directory.Exists(dataPath) || File.Exists(Path.Combine(mod.FullModPath, "modinfo.json")))
                return; 
            // data/ doesn't exist, that's odd
            var foundFolders = Directory.GetDirectories(mod.FullModPath, "data", SearchOption.AllDirectories);
            if (foundFolders.Length > 0)
            {
                // there's a data/ somewhere deeper, probably a mistake then
                mod.Attributes.Add(_factory.Get());
            }
        }
    }
}
