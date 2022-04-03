using Imya.Models;
using Imya.UI.Popup;
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
        #endregion

        public ModActivationView()
        {
            InitializeComponent();
            DataContext = this;
            ModList.ModList_SelectionChanged += ModDescription.SetDisplayedMod;
            ModList.ModList_SelectionChanged += UpdateButtons;
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

            if (Dialog.DialogResult is true)
            {
                await Mods.LoadProfileAsync(Dialog.SelectedProfile);
            }
        }

        private void SaveProfileClick(object sender, RoutedEventArgs e)
        {
            if (Mods is null) return;

            var dialog = new ProfilesSavePopup();

            dialog.ShowDialog();

            if(dialog.DialogResult is true )
            {
                var profile = ModActivationProfile.FromModCollection(ModCollection.Global!, x => x.IsActive);

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

        private void UpdateButtons(Mod? m)
        {
            CanActivate = ModList.AnyInactiveSelected();
            CanDeactivate = ModList.AnyActiveSelected();
            CanDelete = ModList.HasSelection;
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
