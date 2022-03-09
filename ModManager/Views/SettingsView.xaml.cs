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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Imya.Enums;
using Imya.Utils;
using Imya.Models;
using System.ComponentModel;
using Imya.UI.Utils;

namespace Imya.UI.Views
{
    /// <summary>
    /// Interaktionslogik für SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl, INotifyPropertyChanged
    {
        public TextManager TextManager { get; } = TextManager.Instance;
        public SettingsManager SettingsManager { get; } = SettingsManager.Instance;

        public GameSetupManager GameSetupManager { get; } = GameSetupManager.Instance;

        public SettingsView()
        {
            InitializeComponent();
            DataContext = this;

            LanguageSelection.SelectedItem = SettingsManager.Languages.First(x => x.Language == TextManager.Instance.ApplicationLanguage);

            GameSetupManager.GameRootPathChanged += UpdateGameRootPathSetting;
        }

        public void RequestLanguageChange(object sender, RoutedEventArgs e)
        {
            SettingsManager.UpdateLanguage(((ComboBox)sender).SelectedItem);
        }

        public void GameRootPath_ButtonClick(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                GameSetupManager.SetGamePath(dialog.SelectedPath);
            }
        }

        public void UpdateGameRootPathSetting(String newPath)
        {
            Properties.Settings.Default.LANGUAGE_FILE_PATH = newPath;
        }

        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler? PropertyChanged = delegate { };
        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler is PropertyChangedEventHandler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
