
using Anno.EasyMod.Attributes;
using Anno.EasyMod.Mods;

namespace Imya.Models.Attributes
{
    public class ModReplacedByIssue : IModAttribute
    {
        public string AttributeType { get; } = AttributeTypes.ModReplacedByIssue;
        public IText Description { get; init; }

        bool IModAttribute.MultipleAllowed => true;

        readonly IMod _replacedBy; 

        public ModReplacedByIssue(IMod replacedBy)
        {
            _replacedBy = replacedBy;
        }
    }
}
