using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using Imya.Utils;
using Imya.UI.Popup;
using Imya.Models;

namespace Imya.UI.Views
{
    public struct ThemeSetting
    {
        public IText ThemeName { get; private set; }
        public Uri ThemePath { get; private set; }

        public String ThemeID { get; private set; }

        public Brush ThemePrimaryColorBrush { get; private set; }

        public Brush ThemePrimaryColorDarkBrush
        {
            get => new SolidColorBrush(Color.Multiply(_ThemePrimaryColor, 0.5f));
        }

        private Color _ThemePrimaryColor;

        public ThemeSetting(IText name, String path, String id, Color c)
        {
            ThemeName = name;
            ThemePath = new Uri(path, UriKind.RelativeOrAbsolute);
            ThemeID = id;

            _ThemePrimaryColor = c;
            ThemePrimaryColorBrush = new SolidColorBrush(_ThemePrimaryColor);
        }
    }

    public struct LanguageSetting
    {
        public IText LanguageName { get; private set; }
        public ApplicationLanguage Language { get; private set; }

        public LanguageSetting(IText name, ApplicationLanguage lang)
        {
            LanguageName = name;
            Language = lang;
        }
    }

    /// <summary>
    /// Interaktionslogik für SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl, INotifyPropertyChanged
    {
        public TextManager TextManager { get; } = TextManager.Instance;
        public GameSetupManager GameSetupManager { get; } = GameSetupManager.Instance;

        //painfully horrible tbh, this lookup should get better.
        private ResourceDictionary ThemeDictionary = Application.Current.Resources.MergedDictionaries[0];

        public List<ThemeSetting> Themes { get;  } = new List<ThemeSetting>();
        public List<LanguageSetting> Languages { get; } = new List<LanguageSetting>();

        public SettingsView()
        {
            InitializeComponent();

            Themes.Add(new ThemeSetting(TextManager["THEME_GREEN"], "Themes/DarkGreen.xaml", "DarkGreen", Colors.DarkOliveGreen));
            Themes.Add(new ThemeSetting(TextManager["THEME_CYAN"], "Themes/DarkCyan.xaml", "DarkCyan", Colors.DarkCyan));

            Languages.Add(new LanguageSetting(TextManager["SETTINGS_LANG_ENGLISH"], ApplicationLanguage.English));
            Languages.Add(new LanguageSetting(TextManager["SETTINGS_LANG_GERMAN"], ApplicationLanguage.German));

            LanguageSelection.SelectedItem = Languages.First(x => x.Language.ToString().Equals(Properties.Settings.Default.Language));
            ThemeSelection.SelectedItem = Themes.First(x => x.ThemeID.Equals(Properties.Settings.Default.Theme));

            DataContext = this;
        }

        public bool DevMode
        {
            get { return Properties.Settings.Default.DevMode; }
            set
            {
                Properties.Settings.Default.DevMode = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged(nameof(DevMode));
            }
        }

        public bool ModCreatorMode
        { 
            get { return Properties.Settings.Default.ModCreatorMode; }
            set
            {
                Properties.Settings.Default.ModCreatorMode = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged(nameof(ModCreatorMode));    
            }
        }

        public bool ShowConsole
        {
            get { return Properties.Settings.Default.ConsoleVisibility; }
            set
            {
                Properties.Settings.Default.ConsoleVisibility = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged(nameof(ShowConsole));
            }
        }

        public void RequestLanguageChange(object sender, RoutedEventArgs e)
        {
            var box = sender as ComboBox;
            if (box?.SelectedItem is not LanguageSetting pair) return;

            ChangeLanguage(pair);
        }

        public void RequestThemeChange(object sender, RoutedEventArgs e)
        {
            var box = sender as ComboBox;
            if (box?.SelectedItem is not ThemeSetting pair) return;

            ChangeColorTheme(pair);
            
        }

        public void GameRootPath_ButtonClick(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                GameSetupManager.SetGamePath(dialog.SelectedPath);
                // TODO validity feedback?
                Properties.Settings.Default.GameRootPath = dialog.SelectedPath;
                Properties.Settings.Default.Save();
            }
        }

        //Apply new Mod Directory Name
        public void GameModDirectory_ButtonClick(object sender, RoutedEventArgs e)
        {
            String NewName = ModDirectoryNameBox.Text;

            //filter invalid directory names.
            if(NewName.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) >= 0) return;

            //filter if nothing changed
            if (NewName.Equals(Properties.Settings.Default.ModDirectoryName)) return;

            GameSetupManager.SetModDirectoryName(NewName);
            Properties.Settings.Default.ModDirectoryName = NewName;
            Properties.Settings.Default.Save();
        }

        private void ChangeLanguage(LanguageSetting lang)
        {
            TextManager.Instance.ChangeLanguage(lang.Language);
            Properties.Settings.Default.Language = lang.Language.ToString();
            Properties.Settings.Default.Save();
        }

        private void ChangeColorTheme(ThemeSetting theme)
        {
            ResourceDictionary NewTheme = new ResourceDictionary() { Source = theme.ThemePath };

            foreach (var key in NewTheme.Keys)
            {
                ThemeDictionary[key] = NewTheme[key];
            }

            Properties.Settings.Default.Theme = theme.ThemeID;
            Properties.Settings.Default.Save();
        }
        
        public async void OnInstallModLoader(object sender, RoutedEventArgs e)
        {
            InstallationManager installationManager = InstallationManager.Instance;

            Console.WriteLine("Started installing Modloader");
            ModloaderDownloadButton.IsEnabled = false;
            await installationManager.InstallModLoaderAsync();
            ModloaderDownloadButton.IsEnabled = true;

            GenericOkayPopup inputDialog = new GenericOkayPopup();
            inputDialog.ShowDialog();
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

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
