using Imya.Models;
using Imya.Models.Attributes;
using Anno.EasyMod.Mods;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;
using Anno.EasyMod.Attributes;
using Anno.EasyMod.Metadata;

namespace Imya.UI.Models
{
    /// <summary>
    /// Make mod bindable from UI thread.
    /// Direct properties have PropertyChanged notifications.
    /// </summary>
    public class BindableMod : Bindable<IMod>
    {
        public bool IsActive => Model.IsActive;
        public bool IsRemoved => Model.IsRemoved;
        public string Name => Model.Name;

        public BindableCollection<IMod> DistinctSubMods { get; private set; }

        public bool HasSubmods => Model.HasSubmods;
        public Modinfo Modinfo => Model.Modinfo;  

        public BindableCollection<IModAttribute> Attributes { get; private set; }

        public BindableMod(IMod mod, DispatcherObject context) : base(mod, context)
        {
            Attributes = new(Enumerable.Empty<IModAttribute>().ToList(), context);
            DistinctSubMods = new(mod.DistinctSubMods.ToList().AsReadOnly(), context);
        }      
    }
}
