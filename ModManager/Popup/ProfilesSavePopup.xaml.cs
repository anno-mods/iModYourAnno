using Imya.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using System.IO;
using Imya.Utils;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Imya.UI.Popup
{
    /// <summary>
    /// Interaktionslogik für Window1.xaml
    /// </summary>
    public partial class ProfilesSavePopup : Window, INotifyPropertyChanged
    {
        public String ProfileFilename { get; set; } = "Profile1";

        private static String ProfilesDirectoryPath = GameSetupManager.Instance.GetProfilesDirectory();

        public bool IsValidFilename 
        {
            get => _isValidFilename;
            set
            {
                _isValidFilename = value;
                OnPropertyChanged(nameof(IsValidFilename));
            }
        }
        private bool _isValidFilename; 

        public String FullFilename { get => Path.Combine(ProfilesDirectoryPath, ProfileFilename + ".imyaprofile"); }

        public IText OK_TEXT { get; set; }
        public IText CANCEL_TEXT { get; set; }

        public ProfilesSavePopup()
        {
            InitializeComponent();

            OK_TEXT = new SimpleText("OK");
            CANCEL_TEXT = new SimpleText("Cancel");

            DataContext = this;
        }

        private bool ValidateFilename() => !File.Exists(FullFilename);

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            IsValidFilename = ValidateFilename();
        }

        private void OkayButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        { 
            DialogResult = false;
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
