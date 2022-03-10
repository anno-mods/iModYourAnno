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

        public GameSetupManager GameSetupManager { get; } = GameSetupManager.Instance;

        public SettingsView()
        {
            InitializeComponent();
            DataContext = this;

            LanguageSelection.SelectedItem = TextManager.Instance.Languages.First(x => x.Language == TextManager.Instance.ApplicationLanguage);
        }

        public bool DeveloperMode
        { 
            get { return Properties.Settings.Default.DeveloperMode; }
            set
            {
                Properties.Settings.Default.DeveloperMode = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged(nameof(DeveloperMode));    
            }
        }

        public bool ShowConsole
        {
            get { return Properties.Settings.Default.ShowConsole; }
            set
            {
                Properties.Settings.Default.ShowConsole = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged(nameof(ShowConsole));
            }
        }

        public void RequestLanguageChange(object sender, RoutedEventArgs e)
        {
            var box = sender as ComboBox;
            if (box?.SelectedItem is not TextLanguagePair pair) return;

            TextManager.Instance.ChangeLanguage(pair.Language);
            Properties.Settings.Default.Language = pair.Language.ToString();
            Properties.Settings.Default.Save();
        }

        public void GameRootPath_ButtonClick(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                GameSetupManager.SetGamePath(dialog.SelectedPath);
                // TODO validity feedback?
                Properties.Settings.Default.GAME_ROOT_PATH = dialog.SelectedPath;
                Properties.Settings.Default.Save();
            }
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
