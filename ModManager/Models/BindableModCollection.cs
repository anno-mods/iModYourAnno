using Imya.Models;
using Imya.Models.Mods;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;

namespace Imya.UI.Models
{
    /// <summary>
    /// Make mod collection bindable from UI thread.
    /// Direct properties have PropertyChanged notifications.
    /// </summary>
    public class BindableModCollection : BindableCollection<Mod, BindableMod, ModCollection>
    {
        public int ActiveMods => Model.ActiveMods;
        public int ActiveSizeInMBs => Model.ActiveSizeInMBs;
        public int InstalledSizeInMBs => Model.InstalledSizeInMBs;

        public BindableModCollection(ModCollection collection, DispatcherObject context) : base(collection, context, (x, c) => new BindableMod(x, c))
        {
            Order = CompareByActiveCategoryName.Default;
        }
    }
}