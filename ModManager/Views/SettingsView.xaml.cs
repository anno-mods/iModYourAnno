using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Imya.Utils;
using Imya.Models;
using Imya.UI.Views.Components;

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
    /// Mod loader installation, game path, etc.
    /// </summary>
    public partial class SettingsView : BaseControl
    {   
        public GameSetupManager GameSetup { get; } = GameSetupManager.Instance;

        #region Notifiable Properties
        public ModLoaderStatus InstallStatus
        {
            get => _installStatus;
            private set => SetProperty(ref _installStatus, value);
        }
        private ModLoaderStatus _installStatus = ModLoaderStatus.NotInstalled;
        #endregion

        //painfully horrible tbh, this lookup should get better.
        private ResourceDictionary ThemeDictionary = Application.Current.Resources.MergedDictionaries[0];

        public List<ThemeSetting> Themes { get;  } = new List<ThemeSetting>();
        public List<LanguageSetting> Languages { get; } = new List<LanguageSetting>();

        public SettingsView()
        {
            InitializeComponent();

            var textManager = TextManager.Instance;
            Themes.Add(new ThemeSetting(textManager["THEME_GREEN"], "Themes/DarkGreen.xaml", "DarkGreen", Colors.DarkOliveGreen));
            Themes.Add(new ThemeSetting(textManager["THEME_CYAN"], "Themes/DarkCyan.xaml", "DarkCyan", Colors.DarkCyan));

            Languages.Add(new LanguageSetting(TextManager["SETTINGS_LANG_ENGLISH"], ApplicationLanguage.English));
            Languages.Add(new LanguageSetting(TextManager["SETTINGS_LANG_GERMAN"], ApplicationLanguage.German));

            LanguageSelection.SelectedItem = Languages.First(x => x.Language.ToString().Equals(Properties.Settings.Default.Language));
            ThemeSelection.SelectedItem = Themes.First(x => x.ThemeID.Equals(Properties.Settings.Default.Theme));

            DataContext = this;
            textManager.LanguageChanged += OnLanguageChanged;

            if (GameSetup.ModLoader.IsInstalled)
            {
                InstallStatus = ModLoaderStatus.Installed;
                // TODO async update check
                // InstallStatusText = "checking...";
            }
        }

        public bool DevMode
        {
            get => Properties.Settings.Default.DevMode;
            set => SetSetting(value);
        }

        public bool ModCreatorMode
        {
            get => Properties.Settings.Default.ModCreatorMode;
            set => SetSetting(value);
        }

        public bool ConsoleVisibility
        {
            get => Properties.Settings.Default.ConsoleVisibility;
            set => SetSetting(value);
        }

        private void SetSetting<T>(T value, [CallerMemberName] string propertyName = "")
        {
            typeof(Properties.Settings).GetProperty(propertyName)?.SetValue(Properties.Settings.Default, value);
            Properties.Settings.Default.Save();
            OnPropertyChanged(propertyName);
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
                GameSetup.SetGamePath(dialog.SelectedPath);
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

            GameSetup.SetModDirectoryName(NewName);
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
            Console.WriteLine("Installing Modloader");
            ModloaderDownloadButton.IsEnabled = false;
            InstallStatus = ModLoaderStatus.Installing;

            await GameSetup.ModLoader.InstallAsync();

            ModloaderDownloadButton.IsEnabled = true;
            if (GameSetup.ModLoader.IsInstalled)
                InstallStatus = ModLoaderStatus.Installed;
        }

        private void OnLanguageChanged(ApplicationLanguage language)
        {
            // trigger property changes to update text
            OnPropertyChanged(nameof(InstallStatus));
        }
    }

    /// <summary>
    /// Enum with overwritten ToString to provide localized text.
    /// </summary>
    public class ModLoaderStatus
    {
        public static readonly ModLoaderStatus NotInstalled = new("MODLOADER_NOT_INSTALLED");
        public static readonly ModLoaderStatus Checking = new("MODLOADER_CHECKING");
        public static readonly ModLoaderStatus Installing = new("MODLOADER_INSTALLING");
        public static readonly ModLoaderStatus UpdateAvailable = new("MODLOADER_UPDATE_AVAILABLE");
        public static readonly ModLoaderStatus Installed = new("MODLOADER_INSTALLED");

        private readonly string _value;
        private ModLoaderStatus(string value)
        {
            _value = value;
        }

        public IText Localized => TextManager.Instance[_value];
    }
}
