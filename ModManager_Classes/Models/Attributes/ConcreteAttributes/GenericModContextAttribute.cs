using Anno.EasyMod.Attributes;
using Anno.EasyMod.Mods;
using Imya.Utils;

namespace Imya.Models.Attributes
{
    public class GenericModContextAttribute : IModAttribute
    {
        public string AttributeType { get; init; }
        public IText Description { get; init; }
        public IEnumerable<IMod> Context { get; init; }

        bool IModAttribute.MultipleAllowed => true;

        public GenericModContextAttribute()
        { 
            Context = Enumerable.Empty<IMod>();
        }
    }
}
