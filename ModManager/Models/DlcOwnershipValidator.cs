using Imya.Enums;
using Imya.Models;
using Imya.Models.Attributes;
using Imya.Models.Attributes.Factories;
using Imya.Models.Mods;
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

        public void Validate(IEnumerable<Mod> changed, IReadOnlyCollection<Mod> all, NotifyCollectionChangedAction changedAction)
        {
            foreach (Mod m in changed)
            {
                ValidateSingle(m);
            }
        }

        private void ValidateSingle(Mod m)
        {
            m.Attributes.RemoveAttributesByType(AttributeType.DlcNotOwned);
            if (!m.HasDlcDependencies)
                return;

            var potentialDlc = m.Modinfo
                .DLCDependencies?
                .Where(x => x.Dependant != Enums.DlcRequirement.atLeastOneRequired);
            var globallyMissing = _appSettings
                .AllDLCs
                .Where(x => !x.IsEnabled)
                .Select(x => x.DlcId)
                .ToList();
            var missing = potentialDlc?
                .Where(x => globallyMissing.Contains((Enums.DlcId)x.DLC!))
                .Select(x => (DlcId)x.DLC!) 
                ?? Enumerable.Empty<DlcId>();

            if (missing.Count() < 1)
                return;

            m.Attributes.Add(_factory.Get(missing));

        }
    }
}
