using Anno.EasyMod.Mods;
using Anno.EasyMod.Metadata;
using Imya.Models;
using Imya.Models.NotifyPropertyChanged;
using Imya.Models.Options;
using Imya.Utils;
using Imya.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Imya.UI.Models
{
    public struct LanguageSetting
    {
        public string LanguageName { get; private init; }
        public ApplicationLanguage Language { get; private init; }

        public LanguageSetting(IText text, ApplicationLanguage lang)
        {
            var localizedName = text;
            localizedName.Update(lang); // this will globally update them, but it doesn't matter in this specific case
            LanguageName = localizedName.Text;
            Language = lang;
        }
    }

    public struct ThemeSetting
    {
        public IText ThemeName { get; private set; }
        public Uri ThemePath { get; private set; }
        public string ThemeID { get; private set; }
        public Brush ThemePrimaryColorBrush { get; private set; }
        public Brush ThemePrimaryColorDarkBrush { get => new SolidColorBrush(Color.Multiply(_ThemePrimaryColor, 0.5f)); }

        private Color _ThemePrimaryColor;

        public ThemeSetting(IText name, string path, string id, Color c)
        {
            ThemeName = name;
            ThemePath = new Uri(path, UriKind.RelativeOrAbsolute);
            ThemeID = id;

            _ThemePrimaryColor = c;
            ThemePrimaryColorBrush = new SolidColorBrush(_ThemePrimaryColor);
        }
    }

    public struct SortSetting
    {
        public IComparer<IMod> Comparer { get; init; }
        public IText SortingName { get; init; }
        public string ID { get; init; }

        public SortSetting(IComparer<IMod> comparer, IText sortingname, string id)
        {
            Comparer = comparer;
            SortingName = sortingname;
            ID = id;
        }
    }

    public class DlcSetting : PropertyChangedNotifier
    {
        public DlcId DlcId { get; init; }

        public bool IsEnabled 
        {
            get => _isEnabled; 
            set => SetProperty(ref _isEnabled, value);
        }
        private bool _isEnabled; 
    }

    public interface IAppSettings : IDlcOwnershipChanged
    {
        delegate void RateLimitChangedEventHandler(long new_rate_limit);
        delegate void SortSettingChangedEventHandler(SortSetting sortSetting);

        List<ThemeSetting> Themes { get; }
        List<LanguageSetting> Languages { get; }
        List<SortSetting> Sortings { get; }
        IEnumerable<DlcSetting> AllDLCs { get; }

        IModInstallationOptions InstallationOptions { get; }
        bool ShowConsole { get; set; }
        bool ModCreatorMode { get; set; }
        bool DevMode { get; set; }
        bool ModloaderEnabled { get; set; }
        long DownloadRateLimit { get; set; }
        bool UseRateLimiting { get; set; }
        string ModDirectoryName { get; set; }
        string GamePath { get; set; }
        string ModindexLocation { get; set; }

        LanguageSetting Language { get; set; }
        ThemeSetting Theme { get; set; }
        SortSetting Sorting { get; set; }


        event RateLimitChangedEventHandler RateLimitChanged;
        event SortSettingChangedEventHandler SortSettingChanged;

        void Initialize();
    }
}
