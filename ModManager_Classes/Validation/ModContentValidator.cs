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
            if (mod.IsRemoved || !Directory.Exists(mod.FullModPath))
            {
                mod.IsRemoved = true;
                return;
            }
        }
    }
}
