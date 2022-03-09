using Imya.Models;
using Imya.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Imya.UI.Components
{
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

        public ModDirectoryManager ModManager { get; private set; } = ModDirectoryManager.Instance;

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
            
            CurrentlySelectedMod = ListBox_ModList.SelectedItems.Count > 0 ? ListBox_ModList.SelectedItems[ListBox_ModList.SelectedItems.Count -1] as Mod : ListBox_ModList.SelectedItem as Mod;
            CurrentlySelectedMods = selectedItems;
            ModList_SelectionChanged(CurrentlySelectedMod);
        }

        public void ActivateSelection()
        {
            foreach (Mod m in ListBox_ModList.SelectedItems)
            {
                ModManager.Activate(m);
            }
            OnSelectionChanged(); 
        }

        public void DeactivateSelection()
        {
            foreach (Mod m in ListBox_ModList.SelectedItems)
            {
                ModManager.Deactivate(m);
            }
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
            ModDirectoryManager.Instance.FilterMods(x => FilterByKeywords(x, filterText));
        }

        public bool AnyActiveSelected()
        {
            return CurrentlySelectedMods.Any(x => x.Active);
        }

        public bool AnyInactiveSelected()
        {
            return CurrentlySelectedMods.Any(x => !x.Active);
        }

        /// <summary>
        /// Determines whether the category or name of a mod contain the current filter text.
        /// </summary>
        /// <param name="m">Mod to check</param>
        /// <param name="filterText">The boolean result</param>
        /// <returns></returns>
        private bool FilterByNameOrCategory(Mod m, String filterText)
        {
            return m.Name.Text.ToLower().Contains(filterText.ToLower()) || m.Category.Text.ToLower().Contains(filterText.ToLower());
        }

        /// <summary>
        /// Determines whether a mod name+category contains all keywords provided in a search.
        /// </summary>
        /// <param name="m">Mod to check</param>
        /// <param name="filterText">The boolean result</param>
        private bool FilterByKeywords(Mod m, String filterText)
        {
            String[] keywords = filterText.Split(" ");
            bool IsMatch = true;
            foreach (String s in keywords)
            {
                IsMatch = IsMatch ? FilterByNameOrCategory(m, s) : false;
            }
            return IsMatch;
        }

        public event ModListSelectionChangedHandler ModList_SelectionChanged = delegate { };
        public delegate void ModListSelectionChangedHandler(Mod mod);

        #region INotifyPropertyChangedMembers

        public event PropertyChangedEventHandler? PropertyChanged = delegate { };

        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler is PropertyChangedEventHandler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }    
}
