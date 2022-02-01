using Imya.Models.ModMetadata;
using Imya.Utils;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.Serialization;
using Imya.Enums;
using Imya.Models.PropertyChanged;

namespace Imya.Models
{
    public class LocalizedText : PropertyChangedNotifier
    {
        [JsonIgnore]
        public String Text
        {
            get { return _text; }
            private set
            {
                _text = value;
                OnPropertyChanged("Text");
            }
        }
        [JsonIgnore]
        private String _text;

        public String? Chinese { get; set; }
        public String? English { get; set; }
        public String? French { get; set; }
        public String? German { get; set; }
        public String? Italian { get; set; }
        public String? Japanese { get; set; }
        public String? Korean { get; set; }
        public String? Polish { get; set; }
        public String? Russian { get; set; }
        public String? Spanish { get; set; }
        public String? Taiwanese { get; set; }

        public LocalizedText()
        {

        }

        public LocalizedText(Localized localized)
        {
            if (localized.Chinese is String) Chinese = localized.Chinese;
            if (localized.English is String) English = localized.English;
            if (localized.French is String) French = localized.French;
            if (localized.German is String) German = localized.German;
            if (localized.Italian is String) Italian = localized.Italian;
            if (localized.Japanese is String) Japanese = localized.Japanese;
            if (localized.Korean is String) Korean = localized.Korean;
            if (localized.Polish is String) Polish = localized.Polish;
            if (localized.Russian is String) Russian = localized.Russian;
            if (localized.Spanish is String) Spanish = localized.Spanish;
            if (localized.Taiwanese is String) Taiwanese = localized.Taiwanese;

            OnSerialized();
        }

        public LocalizedText(String s)
        { 
            Chinese = s;
            English = s;
            French = s;
            German = s;
            Italian = s;
            Japanese = s;
            Korean = s;
            Polish = s;
            Russian = s;
            Spanish = s;
            Taiwanese = s;

            OnSerialized();
        }

        private void OnSerialized()
        {
            UpdateText(TextManager.Instance.ApplicationLanguage);
            TextManager.Instance.LanguageChanged += UpdateText;
        }

        [OnDeserialized]
        private void OnSerialized(StreamingContext context)
        {
            OnSerialized();
        }

        public void UpdateText(ApplicationLanguage lang)
        {
            switch (lang)
            {
                case ApplicationLanguage.English: if (English is String) Text = English; return;
                case ApplicationLanguage.German: if (German is String) Text = German; return;
            }
            Text = String.Empty;
        }

        public override string ToString()
        {
            return Text;
        }

    }
}
