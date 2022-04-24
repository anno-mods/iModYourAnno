using Imya.Models;
using Imya.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Imya.UI.Components
{
    [ValueConversion(typeof(ModStatus), typeof(string))]
    internal class ModStatusAsIcon : IValueConverter
    {
        static readonly string[] _names = new string[] { "None", "Download", "Update", "RemoveCircleOutline" };

        public object Convert(object value, Type TargetType, object parameter, CultureInfo Culture)
        {
            return _names[(int)value];
        }

        public object ConvertBack(object value, Type TargetType, object parameter, CultureInfo Culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(ModStatus), typeof(string))]
    internal class ModStatusAsColor : IValueConverter
    {
        static readonly string[] _names = new string[] { "Black", "DodgerBlue", "LimeGreen", "Crimson" };

        public object Convert(object value, Type TargetType, object parameter, CultureInfo Culture)
        {
            return _names[(int)value];
        }

        public object ConvertBack(object value, Type TargetType, object parameter, CultureInfo Culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Interaktionslogik für ModList.xaml
    /// </summary>
    public partial class ModList : UserControl, INotifyPropertyChanged
    {
        public IText ActivateText { get; } = TextManager.Instance.GetText("MODLIST_ACTIVATE");
        public IText DeactivateText { get; } = TextManager.Instance.GetText("MODLIST_DEACTIVATE");

        /// <summary>
        /// Either the only or the first mod in the current selection
        /// </summary>
        public Mod? CurrentlySelectedMod { get; private set; } = null;
        public IEnumerable<Mod>? CurrentlySelectedMods { get; private set; } = null;
        public bool HasSelection => CurrentlySelectedMod is not null;

        public ModCollection? Mods { get; private set; } = ModCollection.Global;

        public TextManager TextManager { get; } = TextManager.Instance;

        public ModList()
        {
            InitializeComponent();
            DataContext = this;
            OnSelectionChanged();
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnSelectionChanged();
        }

        private void OnSelectionChanged()
        {
            var selectedItems = ListBox_ModList.SelectedItems.Cast<Mod>().ToList();
            
            CurrentlySelectedMod = ListBox_ModList.SelectedItems.Count > 0 ? 
                GetHighestIndexMod(selectedItems) as Mod :
                ListBox_ModList.SelectedItem as Mod;
            
            
            CurrentlySelectedMods = selectedItems;


            ModList_SelectionChanged?.Invoke(CurrentlySelectedMod);
        }

        private Mod? GetHighestIndexMod(IEnumerable<Mod> mods)
        {
            if (mods.Count() <= 0 || Mods is null) return null;

            Mod? ModHigh = null;
            int IndexHigh = -1;

            foreach (Mod m in mods)
            {
                var i = Mods!.IndexOf(m);
                if (IndexHigh < i)
                {
                    ModHigh = m;
                    IndexHigh = i;
                }
            }

            return ModHigh;
        }

        public async void ActivateSelection()
        {
            foreach (Mod m in ListBox_ModList.SelectedItems)
                await m.ChangeActivationAsync(true);
            OnSelectionChanged(); 
        }

        public async void DeactivateSelection()
        {
            foreach (Mod m in ListBox_ModList.SelectedItems)
                await m.ChangeActivationAsync(false);
            OnSelectionChanged();
        }

        public async void DeleteSelection()
        {
            await Mods!.DeleteAsync(ListBox_ModList.SelectedItems.Cast<Mod>());
            OnSelectionChanged();
        }

        public void ForceSingleSelection()
        { 
            ListBox_ModList.SelectionMode = SelectionMode.Single;
        }

        public void EnableExtendedSelection()
        {
            ListBox_ModList.SelectionMode = SelectionMode.Extended;
        }

        private void OnSearchRequest(object sender, TextChangedEventArgs e)
        {
            string filterText = SearchTextBox.Text;
            ModCollection.Global?.FilterMods(x => x.HasKeywords(filterText));
        }

        public bool AnyActiveSelected()
        {
            return CurrentlySelectedMods?.Any(x => x.IsActive) ?? false;
        }

        public bool AnyInactiveSelected()
        {
            return CurrentlySelectedMods?.Any(x => !x.IsActive) ?? false;
        }

        public event ModListSelectionChangedHandler ModList_SelectionChanged;
        public delegate void ModListSelectionChangedHandler(Mod? mod);

        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler? PropertyChanged = delegate { };
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        private void SetProperty<T>(ref T property, T value, [CallerMemberName] string propertyName = "")
        {
            property = value;
            OnPropertyChanged(propertyName);
        }
        #endregion
    }
}
