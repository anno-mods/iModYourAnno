using Imya.Models;
using Imya.UI.Popup;
using Imya.UI.Utils;
using Imya.Utils;
using System;
using System.ComponentModel;
using System.IO;
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
        public TextManager TextManager { get; } = TextManager.Instance;
        public ModCollection? Mods { get; private set; } = ModCollection.Global;

        #region notifyable properties
        public bool CanActivate
        {
            get => _canActivate;
            private set => SetProperty(ref _canActivate, value);
        }
        private bool _canActivate = false;

        public bool CanDeactivate
        {
            get => _canDeactivate;
            private set => SetProperty(ref _canDeactivate, value);
        }
        private bool _canDeactivate = false;

        public bool CanDelete
        {
            get => _canDelete;
            private set => SetProperty(ref _canDelete, value);
        }
        private bool _canDelete = false;

        public bool CanLoadProfile
        {
            get => _canLoadProfile;
            private set => SetProperty(ref _canLoadProfile, value);
        }
        private bool _canLoadProfile = false;
        #endregion

        public ModActivationView()
        {
            InitializeComponent();
            DataContext = this;
            ModList.ModList_SelectionChanged += ModDescription.SetDisplayedMod;
            ModList.ModList_SelectionChanged += OnUpdateSelection;

            GameSetupManager.Instance.GameStarted += () => UpdateButtons();
            GameSetupManager.Instance.GameClosed += (x, y) => UpdateButtons();
        }

        private void OnGameStart()
        { 
        
        }

        private void OnGameClose(int exitcode, bool RegularClose)
        { 
        
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
            GenericOkayPopup dialog = new();
            dialog.OK_TEXT = new SimpleText("Okay");
            dialog.CANCEL_TEXT = new SimpleText("Cancel");
            dialog.MESSAGE = new SimpleText("This will irreversibly delete all selected mods from the mods folder. Are you sure?");
            dialog.ShowDialog();

            if (dialog.DialogResult is true) ModList.DeleteSelection();
        }

        private async void LoadProfileClick(object sender, RoutedEventArgs e)
        {
            if (Mods is null) return;

            var Dialog = new ProfilesLoadPopup();

            Dialog.ShowDialog();

            if (Dialog.DialogResult is true && Dialog.SelectedProfile is not null)
            {
                await Mods.LoadProfileAsync(Dialog.SelectedProfile);
            }
        }

        private void SaveProfileClick(object sender, RoutedEventArgs e)
        {
            if (Mods is null) return;

            var dialog = new ProfilesSavePopup();

            dialog.ShowDialog();

            if (dialog.DialogResult is true)
            {
                var profile = ModActivationProfile.FromModCollection(Mods, x => x.IsActive);

                if (profile.SaveToFile(dialog.FullFilename))
                {
                    Console.WriteLine($"Saved Profile to {dialog.ProfileFilename}.");
                }
                else
                {
                    Console.WriteLine($"Failed to save profile {dialog.ProfileFilename}.");
                }
            }
        }

        Mod? _previousSelection = null;
        private void OnUpdateSelection(Mod? m)
        {
            if (_previousSelection != m)
            {
                if (_previousSelection is not null)
                    _previousSelection.PropertyChanged -= OnSelectionPropertyChanged;
                if (m is not null)
                    m.PropertyChanged += OnSelectionPropertyChanged;
                _previousSelection = m;
            }
            UpdateButtons();
        }

        private void OnSelectionPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            UpdateButtons();
        }

        private void UpdateButtons()
        { 
            CanActivate = ModList.AnyInactiveSelected() && !GameSetupManager.Instance.IsGameRunning;
            CanDeactivate = ModList.AnyActiveSelected() && !GameSetupManager.Instance.IsGameRunning;
            CanDelete = ModList.HasSelection && !GameSetupManager.Instance.IsGameRunning;

            CanLoadProfile = !GameSetupManager.Instance.IsGameRunning;
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
