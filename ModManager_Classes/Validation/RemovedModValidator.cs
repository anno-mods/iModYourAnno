using Imya.Models.Attributes;
using Imya.Models.Attributes.Interfaces;
using Imya.Models.Mods;
using Imya.Texts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Imya.Validation
{
    public class RemovedModValidator : IModValidator
    {
        private readonly IRemovedFolderAttributeFactory _attributeFactory;
        public RemovedModValidator(IRemovedFolderAttributeFactory attributeFactory) 
        {
            _attributeFactory = attributeFactory;
        }

        public void Validate(IEnumerable<Mod> changed, IReadOnlyCollection<Mod> all)
        {
            foreach (var mod in changed)
                ValidateSingle(mod);
        }


        private void ValidateSingle(Mod mod)
        {
            if (!mod.IsRemoved)
            {
                mod.Attributes.RemoveAttributesByType(AttributeType.IssueModRemoved);
                return;
            }
            mod.Attributes.Clear();
            mod.Attributes.AddAttribute(_attributeFactory.Get());
        }
    }
}
