﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using Imya.Utils;
using Imya.Models;
using System.Threading.Tasks;
using Imya.UI.Popup;
using Imya.Services;
using Imya.Services.Interfaces;
using Imya.Texts;
using Imya.UI.Models;
using System.Windows.Data;
using Imya.UI.Extensions;

namespace Imya.UI.Views
{
    /// <summary>
    /// Mod loader installation, game path, etc.
    /// </summary>
    public partial class SettingsView : UserControl, INotifyPropertyChanged
    {
        public ITextManager TextManager { get; init; }
        public IGameSetupService GameSetup { get; init; }

        public IAppSettings AppSettings { get; init; }

        public long Max { get; } = 100 * 1024 * 1024;
        public long Min { get; } = 256 * 1024;
        public long Stepping { get; } = 256 * 1024;

        public int DlcEntryWidth 
        {
            get => _dlcEntryWidth;
            set 
            {
                _dlcEntryWidth = value;
                OnPropertyChanged(nameof(DlcEntryWidth));
            }
        }
        private int _dlcEntryWidth = 200;

        private int _dlcEntryDesiredWidth = 200; 

        #region Notifiable Properties
        public event PropertyChangedEventHandler? PropertyChanged = delegate { };
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new(propertyName));

        public ModLoaderStatus InstallStatus
        {
            get => _installStatus;
            private set
            {
                _installStatus = value;
                OnPropertyChanged(nameof(InstallStatus));
            }
        }
        private ModLoaderStatus _installStatus = ModLoaderStatus.NotInstalled;
        #endregion

        public SettingsView(
            IAppSettings appSettings,
            ITextManager textManager, 
            IGameSetupService gameSetupService)
        {
            AppSettings = appSettings;
            TextManager = textManager;
            GameSetup = gameSetupService;

            InitializeComponent();

            LanguageSelection.SelectedItem = AppSettings.Language;
            ThemeSelection.SelectedItem = AppSettings.Theme;
            SortingSelection.SelectedItem = AppSettings.Sorting;

            DataContext = this;

            DlcItemsControl.SizeChanged += (sender, e) =>
            {
                var width = e.NewSize.Width;
                var columnCount = (int)width / _dlcEntryDesiredWidth;
                DlcEntryWidth = (int)(width / columnCount) - 5;
            };

            TextManager.LanguageChanged += (lang) =>
            {
                var textsToUpdate = this.FindVisualChildren<TextBlock>(DlcItemsControl);
                foreach (var text in textsToUpdate)
                {
                    var binding = BindingOperations.GetBindingExpression(text, TextBlock.TextProperty);
                    binding?.UpdateTarget(); 
                }
            };
        }

        public void RequestLanguageChange(object sender, RoutedEventArgs e)
        {
            var box = sender as ComboBox;
            if (box?.SelectedItem is not LanguageSetting languageSetting) return;
            AppSettings.Language = languageSetting;
        }

        public void RequestThemeChange(object sender, RoutedEventArgs e)
        {
            var box = sender as ComboBox;
            if (box?.SelectedItem is not ThemeSetting themeSetting) return;
            AppSettings.Theme = themeSetting;
        }

        public void RequestSortChange(object sender, RoutedEventArgs e)
        {
            var box = sender as ComboBox;
            if (box?.SelectedItem is not SortSetting sortSetting) return;
            AppSettings.Sorting = sortSetting;
        }

        //Apply new Mod Directory Name
        public void GameModDirectory_ButtonClick(object sender, RoutedEventArgs e)
        {
            String NewName = ModDirectoryNameBox.Text;

            //filter invalid directory names.
            if(NewName.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) >= 0) return;

            //filter if nothing changed
            if (NewName.Equals(AppSettings.ModDirectoryName)) return;
            AppSettings.ModDirectoryName = NewName;
        }

        public void OnOpenGamePath(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //TODO validity feedback
                AppSettings.GamePath = dialog.SelectedPath;
            }
        }
    }
}
