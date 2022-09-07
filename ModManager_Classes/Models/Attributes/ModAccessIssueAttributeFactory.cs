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
               Description = new SimpleText("Access to the Folder is denied. Please close all programs accessing this folder and retry.")
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
