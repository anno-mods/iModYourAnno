using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using Imya.Utils;
using Imya.Models;
using System.Threading.Tasks;
using Imya.UI.Utils;
using Imya.UI.Popup;

namespace Imya.UI.Views
{
    /// <summary>
    /// Mod loader installation, game path, etc.
    /// </summary>
    public partial class SettingsView : UserControl, INotifyPropertyChanged
    {
        public TextManager TextManager { get; } = TextManager.Instance;
        public GameSetupManager GameSetup { get; } = GameSetupManager.Instance;

        public AppSettings AppSettings { get; set; } = AppSettings.Instance;

        public long Max { get; } = 100 * 1024 * 1024;
        public long Min { get; } = 256 * 1024;
        public long Stepping { get; } = 256 * 1024;

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

        

        public SettingsView()
        {
            InitializeComponent();
            AppSettings.Initialize();

            LanguageSelection.SelectedItem = AppSettings.Language;
            ThemeSelection.SelectedItem = AppSettings.Theme;

            DataContext = this;
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
