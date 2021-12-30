using Imya.Enums;
using Imya.Models;
using Imya.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.UI
{
    public class TextLanguagePair
    {
        public LocalizedText Name { get; set; }
        public ApplicationLanguage Language { get; set; }
    }

    public class SettingsManager : INotifyPropertyChanged
    {
        public static SettingsManager Instance { get; private set; }

        #region FieldBacking
        private bool _showConsole;
        private TextLanguagePair[] _languages;
        private TextLanguagePair _currentLanguage; 
        #endregion

        #region PublicFields
        public bool ShowConsole
        {
            get => _showConsole;
            set
            { 
                _showConsole = value;
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
            var handler = PropertyChanged;
            if (handler is PropertyChangedEventHandler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
