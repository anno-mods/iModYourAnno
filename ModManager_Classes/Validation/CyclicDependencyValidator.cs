using Anno.EasyMod.Mods;
using Imya.Models.Attributes;
using Imya.Models.Attributes.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Validation
{
    public class CyclicDependencyValidator : IModValidator
    {
        private ICyclicDependencyAttributeFactory _attributeFactory;

        public CyclicDependencyValidator(ICyclicDependencyAttributeFactory attributeFactory)
        {
            _attributeFactory = attributeFactory;
        }

        public void Validate(IEnumerable<IMod> changed, IReadOnlyCollection<IMod> all, NotifyCollectionChangedAction changedAction)
        {
            foreach (IMod x in all)
                x.Attributes.RemoveByType(AttributeTypes.CyclicDependency);
            foreach (IMod x in changed)
            {
                var cyclics = CyclicDependencies(x, all);
                if (cyclics.Count() > 0)
                {
                    x.Attributes.Add(_attributeFactory.Get(cyclics));   
                }
            }
        }

        private IEnumerable<IMod> CyclicDependencies(IMod x, IReadOnlyCollection<IMod> others)
        {
            if (!x.IsActive)
                return Enumerable.Empty<IMod>();

            return others.Where(y =>
                y.IsActive && (y.Modinfo?.LoadAfterIds?.Contains(x.ModID) ?? false)
                && (x.Modinfo?.LoadAfterIds?.Contains(y.ModID) ?? false));
        }
    }
}
