using Imya.Models;
using Imya.Models.Attributes;
using Imya.Utils;

namespace Imya.Validation
{
    /// <summary>
    /// Checks if data/ is located in the first subfolder.
    /// That's a common mistake people make.
    /// </summary>
    public class ModContentValidator : IModValidator
    {
        public void Validate(IEnumerable<Mod> changed, IReadOnlyCollection<Mod> all)
        {
            foreach (var mod in changed)
                ValidateSingle(mod);
        }

        private static void ValidateSingle(Mod mod)
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
                    mod.Attributes.AddAttribute(new GenericAttribute() { AttributeType = AttributeType.ModContentInSubfolder, Description = TextManager.Instance.GetText("ATTRIBUTE_MODCONTENTSUBFOLDER") });
                }
            }
        }
    }
}
