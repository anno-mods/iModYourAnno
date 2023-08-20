using Anno.EasyMod.Attributes;
using Anno.EasyMod.Mods;
using Imya.Models.Attributes.Interfaces;
using Imya.Texts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.Attributes.Factories
{
    public class ModReplacedByAttributeFactory : IModReplacedByAttributeFactory
    {
        private ITextManager _textManager;
        public ModReplacedByAttributeFactory(ITextManager textManager)
        {
            _textManager = textManager;
        }

        public IModAttribute Get(IMod replacedBy)
        {
            return new ModReplacedByIssue(replacedBy)
            {
                Description = new SimpleText(string.Format(_textManager.GetText("ATTRIBUTE_REPLACEDBY").Text, replacedBy.FolderName)),
            };
        }
    }
}
