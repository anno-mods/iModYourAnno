using Imya.Models;
using Imya.Models.Attributes;
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
        public BindableCollection<IAttribute> Attributes { get; private set; }

        public BindableMod(Mod mod, DispatcherObject context) : base(mod, context)
        {
            Attributes = new(mod.Attributes, context);
        }      
    }
}
