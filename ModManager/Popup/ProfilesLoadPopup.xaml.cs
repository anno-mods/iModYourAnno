using Imya.Models;
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
        public ObservableCollection<ModActivationProfile> Profiles { get; private set; } = new ObservableCollection<ModActivationProfile>();

        private static String ProfilesDirectoryPath = GameSetupManager.Instance.GetProfilesDirectory();

        public ModActivationProfile SelectedProfile { get; private set; }

        public TextManager TextManager { get; private set; } = TextManager.Instance;

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

        public ProfilesLoadPopup()
        {
            InitializeComponent();
            DataContext = this;

            Title = TextManager.Instance["PROFILE_LOAD"].Text;


            ProfileSelection.SelectionChanged += UpdateSelection;
            Load();

        }

        private void UpdateSelection(object sender, SelectionChangedEventArgs e)
        { 
            HasSelection = ProfileSelection.SelectedIndex >= 0;
        }

        public void Load()
        {
            String ProfilesDirectory = ProfilesDirectoryPath;

            if (!Directory.Exists(ProfilesDirectory))
            {
                Directory.CreateDirectory(ProfilesDirectory);
            }

            foreach (String file in Directory.EnumerateFiles(ProfilesDirectory, "*."+ModActivationProfile.ProfileExtension))
            {
                AddProfile(file);
            }
        }

        public void AddProfile(String Filename)
        {
            ModActivationProfile profile = new ModActivationProfile();
            profile.LoadFromFile(Filename);
            Profiles.Add(profile);
        }

        public void Accept()
        {
            if (ProfileSelection.SelectedIndex != -1)
            {
                SelectedProfile = (ModActivationProfile)ProfileSelection.SelectedItem!;
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

            Button? button = sender as Button;
            ModActivationProfile? profile = button?.DataContext as ModActivationProfile;
            if(profile is not null) DeleteActivationProfile(profile);
        }

        public void DeleteActivationProfile(ModActivationProfile profile)
        {
            Profiles.Remove(profile);
            if (profile.HasFilename)
            {
                File.Delete(profile.Filename!);
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
