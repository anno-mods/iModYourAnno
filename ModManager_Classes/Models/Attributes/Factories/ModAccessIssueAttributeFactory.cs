using Imya.Models.Attributes.Interfaces;
using Imya.Texts;
using Imya.Utils;

namespace Imya.Models.Attributes.Factories
{
    public class ModAccessIssueAttributeFactory : IModAccessIssueAttributeFactory
    {
        private readonly ITextManager _textManager;
        public ModAccessIssueAttributeFactory(ITextManager textManager)
        {
            _textManager = textManager;
        }

        GenericAttribute ModAccessIssueAttribute =
           new GenericAttribute()
           {
               AttributeType = AttributeType.IssueModAccess,
               //Description = TextManager.Instance.GetText("ATTRIBUTE_NOMODINFO")
               Description = new SimpleText("Access to the Folder is denied. Please close all programs accessing this folder and retry.")
           };

        GenericAttribute ModAccessIssue_NoDeleteAttribute =
           new GenericAttribute()
           {
               AttributeType = AttributeType.IssueModAccess,
               //Description = TextManager.Instance.GetText("ATTRIBUTE_NOMODINFO")
               Description = new SimpleText("Could not delete this mod.")
           };

        public IAttribute Get()
        {
            return ModAccessIssueAttribute;
        }

        public IAttribute GetNoDelete()
        {
            return ModAccessIssue_NoDeleteAttribute;
        }
    }
}
