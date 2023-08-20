using Imya.Models.Attributes.Interfaces;
using Imya.Models.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using Anno.EasyMod.Mods;
using Anno.EasyMod.Metadata;

namespace Imya.Validation
{
    public class ModReplacementValidator : IModValidator
    {
        private readonly IModReplacedByAttributeFactory _modReplacedByAttributeFactory;

        public ModReplacementValidator(IModReplacedByAttributeFactory factory)
        {
            _modReplacedByAttributeFactory = factory;
        }

        public void Validate(IEnumerable<IMod> changed, IReadOnlyCollection<IMod> all, NotifyCollectionChangedAction changedAction)
        {
            foreach (var mod in all)
                ValidateSingle(mod, all);
        }

        private void ValidateSingle(IMod mod, IReadOnlyCollection<IMod> collection)
        {
            mod.Attributes.RemoveByType(AttributeTypes.ModReplacedByIssue);
            // skip dependency check if inactive or standalone
            if (!mod.IsActiveAndValid || collection is null)
                return;

            IMod? newReplacementMod = HasBeenDeprecated(mod.Modinfo, collection) ?? IsNewestOfID(mod, collection);
            if (newReplacementMod is not null && newReplacementMod != mod)
                mod.Attributes.Add(_modReplacedByAttributeFactory.Get(newReplacementMod));
        }


        private static IMod? HasBeenDeprecated(Modinfo modinfo, IReadOnlyCollection<IMod> collection)
        {
            if (collection is null || modinfo.ModID is null)
                return null;

            return collection.FirstOrDefault(x => x.Modinfo?.DeprecateIds?.Contains(modinfo.ModID) ?? false);
        }

        private static IMod? IsNewestOfID(IMod mod, IReadOnlyCollection<IMod> collection)
        {
            if (collection is null || mod.Modinfo.ModID is null)
                return null;

            return collection.Where(x => x.Modinfo.ModID == mod.Modinfo.ModID).OrderBy(x => x.Version).LastOrDefault();
        }

    }
}
