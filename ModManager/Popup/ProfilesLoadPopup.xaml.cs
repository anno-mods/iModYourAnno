using Imya.Models;
using Imya.Services;
using Imya.Services.Interfaces;
using Imya.Utils;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Imya.UI.Popup
{
    /// <summary>
    /// Interaktionslogik für ProfilesPopup.xaml
    /// </summary>
    public partial class ProfilesLoadPopup : Window, INotifyPropertyChanged
    {
        public IText OK_TEXT { get; set; }
        public IText CANCEL_TEXT { get; set; }

        public ObservableCollection<String> Profiles { get; private set; }
        public ModActivationProfile? SelectedProfile { get; private set; }

        public bool HasSelection
        {
            get => _hasSelection;
            set
            {
                _hasSelection = value;
                OnPropertyChanged(nameof(HasSelection));
            }
        }
        private bool _hasSelection = false;

        private readonly IProfilesService _profilesService;

        public ProfilesLoadPopup(IProfilesService profilesService)
        {
            _profilesService = profilesService;
            InitializeComponent();
            DataContext = this;
            ProfileSelection.SelectionChanged += UpdateSelection;
            Load();
        }

        private void UpdateSelection(object sender, SelectionChangedEventArgs e)
        { 
            HasSelection = ProfileSelection.SelectedIndex >= 0;
        }

        public void Load() => Profiles = new(_profilesService.GetSavedKeys());

        public void Accept()
        {
            if (ProfileSelection.SelectedIndex != -1)
            {
                SelectedProfile = _profilesService.LoadProfile((String)ProfileSelection.SelectedItem!);
            }
        }

        public void OkayButtonClick(object sender, RoutedEventArgs e)
        {
            Accept();
            DialogResult = true;
        }

        public void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        public void DeleteButtonClick(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Delete requested");

            var button = sender as Button;
            var key = button?.DataContext as String;
            if (key is not null)
            {
                _profilesService.DeleteActivationProfile(key);
                Profiles.Remove(key);
            }
        }

        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler? PropertyChanged = delegate { };
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        protected void SetProperty<T>(ref T property, T value, [CallerMemberName] string propertyName = "")
        {
            property = value;
            OnPropertyChanged(propertyName);
        }
        #endregion
    }
}
