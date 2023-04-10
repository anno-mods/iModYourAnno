using Imya.Models.Attributes;
using Imya.Models.Attributes.Interfaces;
using Imya.Models.Mods;
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

        public void Validate(IEnumerable<Mod> changed, IReadOnlyCollection<Mod> all, NotifyCollectionChangedAction changedAction)
        {
            foreach (var mod in changed)
                ValidateSingle(mod);
        }

        private void ValidateSingle(Mod mod)
        {
            mod.Attributes.RemoveAttributesByType(AttributeType.ModContentInSubfolder);

            if (mod.IsRemoved || !Directory.Exists(mod.FullModPath))
            {
                mod.IsRemoved = true;
                return;
            }

            string dataPath = Path.Combine(mod.FullModPath, "data");
            if (!Directory.Exists(dataPath))
            {
                // data/ doesn't exist, that's odd
                
                var foundFolders = Directory.GetDirectories(mod.FullModPath, "data", SearchOption.AllDirectories);
                if (foundFolders.Length > 0)
                {
                    // there's a data/ somewhere deeper, probably a mistake then
                    mod.Attributes.AddAttribute(_factory.Get());
                }
            }
        }
    }
}
