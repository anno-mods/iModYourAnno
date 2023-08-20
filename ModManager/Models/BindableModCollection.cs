using Anno.EasyMod.Mods;
using Imya.Models;
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
    public class BindableModCollection : BindableCollection<IMod, BindableMod, IModCollection>
    {
        public int ActiveMods => Model.ActiveMods;
        public int ActiveSizeInMBs => Model.ActiveSizeInMBs;
        public int InstalledSizeInMBs => Model.InstalledSizeInMBs;

        public BindableModCollection(IModCollection collection, DispatcherObject context) : base(collection, context, (x, c) => new BindableMod(x, c))
        {
            Order = CompareByActiveCategoryName.Default;
        }
    }
}