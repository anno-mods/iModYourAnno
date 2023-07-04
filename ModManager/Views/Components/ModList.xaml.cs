using Imya.Models;
using Imya.Models.Mods;
using Imya.Services.Interfaces;
using Imya.Texts;
using Imya.UI.Models;
using Imya.Utils;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Imya.UI.Components
{
    /// <summary>
    /// Interaktionslogik für ModList.xaml
    /// </summary>
    public partial class ModList : UserControl, INotifyPropertyChanged
    {
        /// <summary>
        /// Either the only or the first mod in the current selection
        /// </summary>
        public Mod? CurrentlySelectedMod { get; private set; } = null;
        public IEnumerable<Mod>? CurrentlySelectedMods { get; private set; } = null;

        public BindableModCollection Mods { get; init; }

        public ITextManager TextManager { get; init; }
        public IAppSettings Settings { get; init; }

        public ModList(
            ITextManager textManager, 
            IAppSettings settings,
            IImyaSetupService imyaSetupService)
        {
            TextManager = textManager;
            Settings = settings;

            Mods = new BindableModCollection(imyaSetupService.GlobalModCollection, this);

            DataContext = this;
            InitializeComponent();
            OnSelectionChanged();

            Settings.SortSettingChanged += OnSortSettingChanged;
        }

        private void OnSortSettingChanged(SortSetting e) => Mods.Order = e.Comparer;

        public bool ShowAttributes { 
            get => _showAttributes; 
            set => SetProperty(ref _showAttributes, value); 
        }
        private bool _showAttributes = true;

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnSelectionChanged();
        }

        private void OnSelectionChanged()
        {
            var selectedItems = ListBox_ModList.SelectedItems.OfType<BindableMod>();

            CurrentlySelectedMods = selectedItems.Select(x => x.Model).OrderBy(x => x, Mods.Order ?? CompareByActiveCategoryName.Default);
            CurrentlySelectedMod = CurrentlySelectedMods.FirstOrDefault();
            ModList_SelectionChanged?.Invoke(CurrentlySelectedMod);
        }

        public async void ActivateSelection()
        {
            var selected = ListBox_ModList.SelectedItems.OfType<BindableMod>().Select(x => x.Model).ToArray();
            await Mods.Model.ChangeActivationAsync(selected, true);

            OnSelectionChanged(); 
        }

        public async void DeactivateSelection()
        {
            var selected = ListBox_ModList.SelectedItems.OfType<BindableMod>().Select(x => x.Model).ToArray();
            await Mods.Model.ChangeActivationAsync(selected, false);

            OnSelectionChanged();
        }

        public async void DeleteSelection()
        {
            await Mods.Model.DeleteAsync(ListBox_ModList.SelectedItems.OfType<BindableMod>().Select(x => x.Model).ToArray());
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
            Mods.Filter = string.IsNullOrWhiteSpace(filterText) ? null : x => x.HasKeywords(filterText);
        }

        public event ModListSelectionChangedHandler? ModList_SelectionChanged;
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

        private void ButtonExpandEverythingClick(object sender, System.Windows.RoutedEventArgs e)
        {
            SetAllExpandersExpandedStatusTo(true);
        }

        private void ButtonCollapseEverythingClick(object sender, System.Windows.RoutedEventArgs e)
        {
            SetAllExpandersExpandedStatusTo(false);
        }

        private void SetAllExpandersExpandedStatusTo(bool to)
        {
            var expanders = FindVisualChildren<Expander>(ListBox_ModList).ToArray();

            foreach (Expander expander in expanders)
            {
                if(expander.IsExpanded != to)
                    expander.IsExpanded = to;
            }
        }

        //see https://learn.microsoft.com/de-de/dotnet/desktop/wpf/data/how-to-find-datatemplate-generated-elements?view=netframeworkdesktop-4.8
        private IEnumerable<childItem> FindVisualChildren<childItem>(DependencyObject obj) where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                {
                    yield return (childItem)child;
                }
                else
                {
                    var grandchildren = FindVisualChildren<childItem>(child);
                    foreach (var grandchild in grandchildren)
                    {
                        if (grandchild != null)
                            yield return grandchild;
                    }
                }
            }
        }
    }
}
