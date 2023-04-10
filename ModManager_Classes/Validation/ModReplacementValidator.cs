using Imya.Models.Attributes.Interfaces;
using Imya.Models.Attributes;
using Imya.Models.Mods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imya.Models.ModMetadata.ModinfoModel;
using System.Collections.Specialized;

namespace Imya.Validation
{
    public class ModReplacementValidator : IModValidator
    {
        private readonly IModReplacedByAttributeFactory _modReplacedByAttributeFactory;

        public ModReplacementValidator(IModReplacedByAttributeFactory factory)
        {
            _modReplacedByAttributeFactory = factory;
        }

        public void Validate(IEnumerable<Mod> changed, IReadOnlyCollection<Mod> all, NotifyCollectionChangedAction changedAction)
        {
            foreach (var mod in all)
                ValidateSingle(mod, all);
        }

        private void ValidateSingle(Mod mod, IReadOnlyCollection<Mod> collection)
        {
            mod.Attributes.RemoveAttributesByType(AttributeType.ModReplacedByIssue);
            // skip dependency check if inactive or standalone
            if (!mod.IsActiveAndValid || collection is null)
                return;

            Mod? newReplacementMod = HasBeenDeprecated(mod.Modinfo, collection) ?? IsNewestOfID(mod, collection);
            if (newReplacementMod is not null && newReplacementMod != mod)
                mod.Attributes.AddAttribute(_modReplacedByAttributeFactory.Get(newReplacementMod));
        }


        private Mod? HasBeenDeprecated(Modinfo modinfo, IReadOnlyCollection<Mod> collection)
        {
            if (collection is null || modinfo.ModID is null)
                return null;

            return collection.FirstOrDefault(x => x.Modinfo?.DeprecateIds?.Contains(modinfo.ModID) ?? false);
        }

        private static Mod? IsNewestOfID(Mod mod, IReadOnlyCollection<Mod> collection)
        {
            if (collection is null || mod.Modinfo.ModID is null)
                return null;

            return collection.Where(x => x.Modinfo.ModID == mod.Modinfo.ModID).OrderBy(x => x.Version).LastOrDefault();
        }

    }
}
