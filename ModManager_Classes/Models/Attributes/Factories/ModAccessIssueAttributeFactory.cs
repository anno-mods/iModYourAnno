﻿using Anno.EasyMod.Attributes;
using Imya.Models.Attributes.Interfaces;
using Imya.Texts;

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
               AttributeType = AttributeTypes.IssueModAccess,
               //Description = TextManager.Instance.GetText("ATTRIBUTE_NOMODINFO")
               Description = new SimpleText("Access to the Folder is denied. Please close all programs accessing this folder and retry.")
           };

        GenericAttribute ModAccessIssue_NoDeleteAttribute =
           new GenericAttribute()
           {
               AttributeType = AttributeTypes.IssueModAccess,
               //Description = TextManager.Instance.GetText("ATTRIBUTE_NOMODINFO")
               Description = new SimpleText("Could not delete this mod.")
           };

        public IModAttribute Get()
        {
            return ModAccessIssueAttribute;
        }

        public IModAttribute GetNoDelete()
        {
            return ModAccessIssue_NoDeleteAttribute;
        }
    }
}
