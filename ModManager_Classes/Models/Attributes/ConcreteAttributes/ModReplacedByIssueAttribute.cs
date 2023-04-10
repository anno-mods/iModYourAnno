using Imya.Models.Mods;
using Imya.Utils;

namespace Imya.Models.Attributes
{
    public class ModReplacedByIssue : IAttribute
    {
        public AttributeType AttributeType { get; } = AttributeType.ModReplacedByIssue;
        public IText Description { get => new SimpleText(string.Format(TextManager.Instance.GetText("ATTRIBUTE_REPLACEDBY").Text, _replacedBy.FolderName)); }

        bool IAttribute.MultipleAllowed => true;

        readonly Mod _replacedBy; 

        public ModReplacedByIssue(Mod replacedBy)
        {
            _replacedBy = replacedBy;
        }
    }
}
