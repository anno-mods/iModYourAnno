using Anno.EasyMod.Mods;
using Imya.Models;
using Imya.Services;
using Imya.Services.Interfaces;
using Imya.Texts;
using Imya.UI.Components;
using Imya.UI.Popup;
using Imya.UI.Utils;
using Imya.Utils;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Imya.UI.Views
{
    /// <summary>
    /// Interaktionslogik für ModActivationView.xaml
    /// </summary>
    public partial class ModActivationView : UserControl, INotifyPropertyChanged
    {
        public ITextManager TextManager { get; init; }
        public IModCollection? Mods { get; private set; }
        public IGameSetupService GameSetupManager { get; }

        public ModList ModList { get; init; }
        public ModDescriptionDisplay ModDescription { get; init; }

        private PopupCreator _popupCreator;
        private IProfilesService _profilesService;

        #region notifyable properties

        public bool AnyActiveSelected
        {
            get => _anyActiveSelected;
            set => SetProperty(ref _anyActiveSelected, value);
        }
        private bool _anyActiveSelected;

        public bool AnyInactiveSelected
        {
            get => _anyInactiveSelected;
            set => SetProperty(ref _anyInactiveSelected, value);
        }
        private bool _anyInactiveSelected;

        public bool OnlyRemovedSelected
        {
            get => _onlyRemovedSelected;
            set => SetProperty(ref _onlyRemovedSelected, value);
        }
        private bool _onlyRemovedSelected;

        public bool HasSelection
        {
            get => _hasSelection;
            set => SetProperty(ref _hasSelection, value);
        }
        private bool _hasSelection;

        #endregion

        public ModActivationView(
            IGameSetupService gameSetup,
            ITextManager textManager,
            ModList modList,
            ModDescriptionDisplay modDescriptionDisplay,
            IImyaSetupService imyaSetupService,
            PopupCreator popupCreator,
            IProfilesService profilesService)
        {
            GameSetupManager = gameSetup;
            TextManager = textManager;
            Mods = imyaSetupService.GlobalModCollection;
            ModList = modList; 
            ModDescription = modDescriptionDisplay;
            _popupCreator = popupCreator;
            _profilesService = profilesService;

            DataContext = this;
            InitializeComponent();

            ModList.ModList_SelectionChanged += ModDescription.SetDisplayedMod;
            ModList.ModList_SelectionChanged += OnUpdateSelection;
        }

        private void OnActivate(object sender, RoutedEventArgs e)
        {
            ModList.ActivateSelection();
        }

        private void OnDeactivate(object sender, RoutedEventArgs e)
        {
            ModList.DeactivateSelection();
        }

        private void OnDelete(object sender, RoutedEventArgs e)
        {
            bool hasModsWorthyAsking = ModList.CurrentlySelectedMods?.Any(x => !x.IsRemoved) ?? false;
            bool deleteMods = !hasModsWorthyAsking;

            if (hasModsWorthyAsking)
            {
                GenericOkayPopup dialog = new()
                {
                    OK_TEXT = new SimpleText("Okay"),
                    CANCEL_TEXT = new SimpleText("Cancel"),
                    MESSAGE = new SimpleText("This will irreversibly delete all selected mods from the mods folder. Are you sure?")
                };
                dialog.ShowDialog();

                deleteMods = dialog.DialogResult ?? false;
            }

            if (deleteMods)
                ModList.DeleteSelection();
        }

        private async void LoadProfileClick(object sender, RoutedEventArgs e)
        {
            if (Mods is null) return;

            var Dialog = _popupCreator.CreateProfilesLoadPopup();
            var dialogResult = Dialog.ShowDialog();

            if (dialogResult is true && Dialog.SelectedProfile is not null)
            {
                //await Mods.LoadProfileAsync(Dialog.SelectedProfile);
            }
        }

        private void SaveProfileClick(object sender, RoutedEventArgs e)
        {
            if (Mods is null) return;

            var dialog = _popupCreator.CreateProfilesSavePopup();

            dialog.ShowDialog();
            if (dialog.DialogResult is not true)
                return;

            var profile = _profilesService.CreateFromModCollection(Mods);
            _profilesService.SaveProfile(profile, dialog.ProfileFilename);
        }

        IMod? _previousSelection = null;
        private void OnUpdateSelection(IMod? m)
        {
            if (_previousSelection == m)
                return;

            HasSelection = ModList.CurrentlySelectedMod is not null;
            AnyActiveSelected = ModList.CurrentlySelectedMods?.Any(x => x.IsActive) ?? false;
            AnyInactiveSelected = ModList.CurrentlySelectedMods?.Any(x => !x.IsActive) ?? false;
            OnlyRemovedSelected = ModList.CurrentlySelectedMods?.Where(x => x.IsRemoved).Count() == ModList.CurrentlySelectedMods?.Count();



            _previousSelection = m;
        }

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
