using Imya.Enums;
using Imya.Models;
using Imya.Models.PropertyChanged;
using Imya.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Imya.UI.Properties;

namespace Imya.UI.Utils
{
    public class TextLanguagePair
    {
        public IText Name { get; set; }
        public ApplicationLanguage Language { get; set; }
    }

    public class SettingsManager : INotifyPropertyChanged
    {
        public static SettingsManager Instance { get; private set; }

        #region FieldBacking
        private bool _showConsole;
        private TextLanguagePair[] _languages;
        private TextLanguagePair _currentLanguage;
        private bool _dev_mode;
        #endregion

        #region PublicFields
        public bool ShowConsole
        {
            get => Settings.Default.SHOW_CONSOLE;
            set
            { 
                Settings.Default.SHOW_CONSOLE = value;
                OnPropertyChanged(nameof(ShowConsole));
            }
        }
        public TextLanguagePair CurrentLanguage
        {
            get => _currentLanguage;
            set
            {
                _currentLanguage = value;
                OnPropertyChanged(nameof(CurrentLanguage));
            }
        }
        public TextLanguagePair[] Languages
        {
            get => _languages;
            set
            {
                _languages = value;
                OnPropertyChanged(nameof(Languages));
            }
        }
        public bool DevMode
        {
            get => Settings.Default.ENABLE_DEV_FEATURES;
            set
            {
                Settings.Default.ENABLE_DEV_FEATURES = value;
                OnPropertyChanged(nameof(DevMode));
            }
        }
        #endregion

        public SettingsManager()
        {
            Instance = Instance ?? this;

            Languages = new TextLanguagePair[]
            {
                new TextLanguagePair() { Name = TextManager.Instance.GetText("SETTINGS_LANG_ENGLISH"), Language = ApplicationLanguage.English},
                new TextLanguagePair() { Name = TextManager.Instance.GetText("SETTINGS_LANG_GERMAN"), Language = ApplicationLanguage.German }
            };

            ShowConsole = true;
        }

        //this will get a TextLanguagePair from the ComboBox.
        public void UpdateLanguage(object t)
        {
            if (t is not TextLanguagePair) throw new ArgumentException(nameof(t));
            TextManager.Instance.ChangeLanguage(((TextLanguagePair)t).Language);
            CurrentLanguage = (TextLanguagePair)t;
        }

        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler? PropertyChanged = delegate { };
        private void OnPropertyChanged(string propertyName)
        {
            this.NotifyPropertyChanged(PropertyChanged, propertyName);
        }
        #endregion
    }
}
