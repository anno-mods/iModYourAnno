using Imya.Models;
using Imya.Models.NotifyPropertyChanged;
using Imya.Models.Options;
using Imya.Services;
using Imya.Services.Interfaces;
using Imya.Texts;
using Imya.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Imya.UI.Models
{

    public class AppSettings : PropertyChangedNotifier, IAppSettings
    {
        private readonly ITextManager TextManager;
        private readonly IGameSetupService GameSetup;

        public List<ThemeSetting> Themes { get; } = new List<ThemeSetting>();
        public List<LanguageSetting> Languages { get; } = new List<LanguageSetting>();
        public List<SortSetting> Sortings { get; } = new List<SortSetting>();

        //painfully horrible tbh, this lookup should get better.
        private ResourceDictionary ThemeDictionary;

        public IModInstallationOptions InstallationOptions { get; }

        public bool ShowConsole
        {
            get => Properties.Settings.Default.ConsoleVisibility;
            set
            {
                Properties.Settings.Default.ConsoleVisibility = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged(nameof(ShowConsole));
            }
        }

        public bool ModCreatorMode
        {
            get => Properties.Settings.Default.ModCreatorMode;
            set
            {
                Properties.Settings.Default.ModCreatorMode = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged(nameof(ModCreatorMode));
            }
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

        public string ModDirectoryName
        {
            get => Properties.Settings.Default.ModDirectoryName;
            set
            {
                GameSetup.SetModDirectoryName(value);
                Properties.Settings.Default.ModDirectoryName = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged(nameof(ModDirectoryName));
            }
        }

        public string GamePath
        {
            get => Properties.Settings.Default.GameRootPath;
            set
            {
                GameSetup.SetGamePath(value, true);
                Properties.Settings.Default.GameRootPath = GameSetup.GameRootPath;
                Properties.Settings.Default.Save();
                OnPropertyChanged(nameof(GamePath));
            }
        }

        public string ModindexLocation
        {
            get => Properties.Settings.Default.ModindexLocation;
            set
            {
                Properties.Settings.Default.ModindexLocation = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged(nameof(ModindexLocation));
            }
        }

        public bool ModloaderEnabled
        {
            get => Properties.Settings.Default.ModloaderEnabled;
            set
            {
                Properties.Settings.Default.ModloaderEnabled = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged(nameof(ModloaderEnabled));
            }
        }

        public LanguageSetting Language
        {
            get { return _language; }
            set
            {
                if (!Languages.Contains(value))
                    throw new InvalidOperationException("Language not part of the languages list");
                ChangeLanguage(value);
                _language = value;
                OnPropertyChanged(nameof(Language));
            }
        }
        private LanguageSetting _language;

        public ThemeSetting Theme
        {
            get { return _theme; }
            set
            {
                if (!Themes.Contains(value))
                    throw new InvalidOperationException("Theme not part of the themes list");
                ChangeColorTheme(value);
                _theme = value;
                OnPropertyChanged(nameof(Theme));
            }
        }
        private ThemeSetting _theme;

        public SortSetting Sorting
        {
            get => _sorting;
            set
            {
                if (!Sortings.Contains(value))
                    throw new InvalidOperationException("Sorting not part of the sortings list");
                Properties.Settings.Default.Sorting = value.ID;
                Properties.Settings.Default.Save();
                _sorting = value;
                OnPropertyChanged(nameof(Sorting));
                SortSettingChanged.Invoke(value);
            }
        }
        private SortSetting _sorting;

        public long DownloadRateLimit
        {
            get => Properties.Settings.Default.DownloadRateLimit;
            set
            {
                Properties.Settings.Default.DownloadRateLimit = value;
                Properties.Settings.Default.Save();
                RateLimitChanged(UseRateLimiting ? value : 0);
                OnPropertyChanged(nameof(DownloadRateLimit));
            }
        }

        public bool UseRateLimiting
        {
            get => Properties.Settings.Default.UseRatelimit;
            set
            {
                Properties.Settings.Default.UseRatelimit = value;
                Properties.Settings.Default.Save();
                RateLimitChanged(value ? DownloadRateLimit : 0);
                OnPropertyChanged(nameof(UseRateLimiting));
            }
        }

        public event IAppSettings.RateLimitChangedEventHandler RateLimitChanged = delegate { };
        public event IAppSettings.SortSettingChangedEventHandler SortSettingChanged = delegate { };

        public AppSettings(
            IInstallationService installationService,
            ITextManager textManager,
            IGameSetupService gameSetupService)
        {
            TextManager = textManager;
            GameSetup = gameSetupService;

            Themes.Add(new ThemeSetting(TextManager["THEME_GREEN"], "Styles/Themes/DarkGreen.xaml", "DarkGreen", Colors.DarkOliveGreen));
            Themes.Add(new ThemeSetting(TextManager["THEME_CYAN"], "Styles/Themes/DarkCyan.xaml", "DarkCyan", Colors.DarkCyan));
            Themes.Add(new ThemeSetting(TextManager["THEME_LIGHT"], "Styles/Themes/Light.xaml", "Light", Colors.LightGray));
            Themes.Add(new ThemeSetting(TextManager["THEME_DARKVIOLET"], "Styles/Themes/DarkViolet.xaml", "DarkViolet", Colors.Purple));

            Languages.Add(new LanguageSetting("SETTINGS_LANG_ENGLISH", ApplicationLanguage.English));
            Languages.Add(new LanguageSetting("SETTINGS_LANG_GERMAN", ApplicationLanguage.German));

            Sortings.Add(new SortSetting(CompareByActiveCategoryName.Default, TextManager["SORTING_DEFAULT"], "Default"));
            Sortings.Add(new SortSetting(CompareByCategoryName.Default, TextManager["SORTING_ACTIVE_AGNOSTIC"], "ActiveAgnostic"));
            Sortings.Add(new SortSetting(CompareByFolder.Default, TextManager["SORTING_BYFOLDER"], "Folder"));
            Sortings.Add(new SortSetting(ComparebyLoadOrder.Default, TextManager["SORTING_LOADORDER"], "LoadOrder"));

            RateLimitChanged += x => installationService.DownloadConfig.MaximumBytesPerSecond = x;
        }

        private void ChangeLanguage(LanguageSetting lang)
        {
            Properties.Settings.Default.Language = lang.Language.ToString();
            Properties.Settings.Default.Save();
            TextManager.ChangeLanguage(lang.Language);
        }

        private void ChangeColorTheme(ThemeSetting theme)
        {
            Properties.Settings.Default.Theme = theme.ThemeID;
            Properties.Settings.Default.Save();

            ResourceDictionary NewTheme = new ResourceDictionary() { Source = theme.ThemePath };
            foreach (var key in NewTheme.Keys)
            {
                ThemeDictionary[key] = NewTheme[key];
            }
        }

        private void LoadSortSetting()
        {
            Sorting = Sortings.FirstOrDefault(x => x.ID.ToString().Equals(Properties.Settings.Default.Sorting), Sortings[0]);
        }

        private void LoadLanguageSetting()
        {
            Language = Languages.FirstOrDefault(x => x.Language.ToString().Equals(Properties.Settings.Default.Language), Languages[0]);
        }

        private void LoadThemeSetting()
        {
            Theme = Themes.FirstOrDefault(x => x.ThemeID.Equals(Properties.Settings.Default.Theme), Themes[0]);
        }

        /// <summary>
        /// We need to defer the loading of ThemeDictionary until the app has started.
        /// </summary>
        public void Initialize()
        {
            ThemeDictionary = Application.Current.Resources.MergedDictionaries[0];
            LoadLanguageSetting();
            LoadThemeSetting();
            LoadSortSetting();
        }
    }
}
