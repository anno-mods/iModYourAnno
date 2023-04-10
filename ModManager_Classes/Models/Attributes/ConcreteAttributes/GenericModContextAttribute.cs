using Imya.Models.Mods;
using Imya.Utils;

namespace Imya.Models.Attributes
{
    public class GenericModContextAttribute : IAttribute
    {
        public AttributeType AttributeType { get; init; }
        public IText Description { get; init; }
        public IEnumerable<Mod> Context { get; init; }

        bool IAttribute.MultipleAllowed => true;

        public GenericModContextAttribute()
        { 
            Context = Enumerable.Empty<Mod>();
        }
    }
}
