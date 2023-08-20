using Anno.EasyMod.Mods;
using Imya.Models.Attributes;
using Imya.Models.Attributes.Interfaces;
using Imya.Texts;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

        public void Validate(IEnumerable<IMod> changed, IReadOnlyCollection<IMod> all, NotifyCollectionChangedAction changedAction)
        {
            foreach (var mod in changed)
                ValidateSingle(mod);
        }


        private void ValidateSingle(IMod mod)
        {
            if (!mod.IsRemoved)
            {
                mod.Attributes.RemoveByType(AttributeTypes.IssueModRemoved);
                return;
            }
            mod.Attributes.Clear();
            mod.Attributes.Add(_attributeFactory.Get());
        }
    }
}
