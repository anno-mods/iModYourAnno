using Imya.Models;
using Imya.UI.Models;
using Imya.UI.Utils;
using Imya.Utils;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

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

        public BindableModCollection Mods { get; init; }

        public TextManager TextManager { get; } = TextManager.Instance;

        public AppSettings Settings { get; } = AppSettings.Instance;

        public ModList()
        {
            Mods = new BindableModCollection(ModCollection.Global ?? ModCollection.Empty, this);

            InitializeComponent();
            DataContext = this;
            OnSelectionChanged();

            Settings.PropertyChanged += Settings_PropertyChanged;
        }

        private void Settings_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Settings.Sorting))
            {
                Mods.Order = Settings.Sorting.Comparer;
            }
        }

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
    }
}
