using Imya.Models;
using Imya.Utils;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace Imya.UI.Popup
{
    /// <summary>
    /// Interaktionslogik für ProfilesPopup.xaml
    /// </summary>
    public partial class ProfilesLoadPopup : Window
    {
        public ObservableCollection<ModActivationProfile> Profiles { get; private set; } = new ObservableCollection<ModActivationProfile>();

        private static String ProfilesDirectoryName = "profiles";

        public ModActivationProfile SelectedProfile { get; private set; }

        public TextManager TextManager { get; private set; } = TextManager.Instance;

        public ProfilesLoadPopup()
        {
            InitializeComponent();
            DataContext = this;

            Load();
        }

        public void Load()
        {
            String ProfilesDirectory = Path.Combine(GameSetupManager.Instance.GameRootPath, ProfilesDirectoryName);
            
            if (!Directory.Exists(ProfilesDirectory))
            { 
                Directory.CreateDirectory(ProfilesDirectory);
            }

            foreach (String file in Directory.EnumerateFiles(ProfilesDirectory, "*.imyaprofile"))
            {
                AddProfile(file);
            }
        }

        public void AddProfile(String Filename)
        {
            
            ModActivationProfile profile = new ModActivationProfile();
            profile.LoadFromFile(Filename);

            if (profile.IsEmpty()) return;

            String profileName = Path.GetFileNameWithoutExtension(Filename);
            profile.Title = profileName;

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
    }
}
