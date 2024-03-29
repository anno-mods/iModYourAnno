﻿using Imya.Models;
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
using Imya.Services;
using Imya.Services.Interfaces;

namespace Imya.UI.Popup
{
    public enum FilenameValidation
    { 
        Valid, 
        AlreadyExists,
        Invalid
    }
    /// <summary>
    /// Interaktionslogik für Window1.xaml
    /// </summary>
    public partial class ProfilesSavePopup : Window, INotifyPropertyChanged
    {
        public String ProfileFilename { get; set; } = "Profile1";
        public String ProfilesDirectoryPath { get; init; }

        public bool IsValidFilename
        {
            get => _isValidFilename;
            private set
            {
                _isValidFilename = value;
                OnPropertyChanged(nameof(IsValidFilename));
            }
        }
        private bool _isValidFilename;

        public FilenameValidation FilenameValidation { get => _filenameValidation;
            set
            {
                IsValidFilename = value == FilenameValidation.Valid;
                _filenameValidation = value;
                OnPropertyChanged(nameof(FilenameValidation));
            }
        }
        private FilenameValidation _filenameValidation;

        public IText OK_TEXT { get; set; }
        public IText CANCEL_TEXT { get; set; }

        private readonly IProfilesService _profilesService; 

        public ProfilesSavePopup(IProfilesService profilesService)
        {
            _profilesService = profilesService; 
            InitializeComponent();
            DataContext = this;
            NameTextbox.TextChanged += FilenameChanged;
        }

        private FilenameValidation ValidateFilename()
        {
            char[] Invalids = Path.GetInvalidFileNameChars();

            if (ProfileFilename.Any(x => Invalids.Contains(x))) 
                return FilenameValidation.Invalid;
            if (_profilesService.ProfileExists(ProfileFilename)) 
                return FilenameValidation.AlreadyExists;

            return FilenameValidation.Valid;
        }

        private void FilenameChanged(object sender, TextChangedEventArgs e)
        {
            FilenameValidation = ValidateFilename();
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
