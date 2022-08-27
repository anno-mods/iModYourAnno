using Imya.Models;
using Imya.Models.Attributes;

namespace Imya.Utils.Validation
{
    /// <summary>
    /// Checks if data/ is located in the first subfolder.
    /// That's a common mistake people make.
    /// </summary>
    public class ModContentValidator : IModValidator
    {
        public void Validate(Mod mod)
        {
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
