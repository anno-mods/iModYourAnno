using Anno.EasyMod.Mods;
using Anno.EasyMod.Metadata;
using Imya.Models;
using Imya.Models.Attributes;
using Imya.Models.Attributes.Factories;
using Imya.Validation;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.UI.Models
{
    internal class DlcOwnershipValidator : IModValidator
    {
        private IAppSettings _appSettings;
        private DlcOwnershipAttributeFactory _factory; 
        public DlcOwnershipValidator(IAppSettings appSettings,
            DlcOwnershipAttributeFactory factory) 
        {
            _appSettings = appSettings;
            _factory = factory; 
        }

        public void Validate(IEnumerable<IMod> changed, IReadOnlyCollection<IMod> all, NotifyCollectionChangedAction changedAction)
        {
            foreach (IMod m in all)
            {
                ValidateSingle(m);
            }
        }

        private void ValidateSingle(IMod m)
        {
            m.Attributes.RemoveByType(AttributeTypes.DlcNotOwned);
            if (m.Modinfo.DLCDependencies is null)
                return;

            var potentialDlc = m.Modinfo
                .DLCDependencies?
                .Where(x => x.Dependant != DlcRequirement.atLeastOneRequired && x.DLC is not null && x.Dependant is not null);
            var globallyMissing = _appSettings
                .AllDLCs
                .Where(x => !x.IsEnabled)
                .Select(x => x.DlcId)
                .ToList();
            var missing = potentialDlc?
                .Where(x => globallyMissing.Contains((DlcId)x.DLC!))
                .Select(x => (DlcId)x.DLC!) 
                ?? Enumerable.Empty<DlcId>();

            if (missing.Count() < 1)
                return;

            m.Attributes.Add(_factory.Get(missing));

        }
    }
}
