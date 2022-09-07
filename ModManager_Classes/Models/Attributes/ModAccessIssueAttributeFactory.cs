using Imya.Utils;

namespace Imya.Models.Attributes
{
    internal class ModAccessIssueAttributeFactory
    {
        static GenericAttribute ModAccessIssueAttribute =
           new GenericAttribute()
           {
               AttributeType = AttributeType.IssueModAccess,
               //Description = TextManager.Instance.GetText("ATTRIBUTE_NOMODINFO")
               Description = IText.Empty
           };

        static GenericAttribute ModAccessIssue_NoDeleteAttribute =
           new GenericAttribute()
           {
               AttributeType = AttributeType.IssueModAccess,
               //Description = TextManager.Instance.GetText("ATTRIBUTE_NOMODINFO")
               Description = new SimpleText("Could not delete this mod.")
           };

        public static IAttribute Get()
        {
            return ModAccessIssueAttribute;
        }

        public static IAttribute GetNoDelete()
        {
            return ModAccessIssue_NoDeleteAttribute;
        }
    }
}
