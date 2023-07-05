using Imya.Models;
using Imya.Models.Attributes;
using Imya.Models.Mods;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;

namespace Imya.UI.Models
{
    /// <summary>
    /// Make mod bindable from UI thread.
    /// Direct properties have PropertyChanged notifications.
    /// </summary>
    public class BindableMod : Bindable<Mod>
    {
        public bool IsActive => Model.IsActive;
        public bool IsRemoved => Model.IsRemoved;
        public IText Name => Model.Name;
        public IText Category => Model.Category;

        public BindableCollection<Mod> DistinctSubMods { get; private set; }

        public bool HasSubmods => Model.HasSubmods;

        public BindableCollection<IAttribute> Attributes { get; private set; }

        public BindableMod(Mod mod, DispatcherObject context) : base(mod, context)
        {
            Attributes = new(mod.Attributes, context);
            DistinctSubMods = new(mod.DistinctSubMods.ToList().AsReadOnly(), context);
        }      
    }
}
