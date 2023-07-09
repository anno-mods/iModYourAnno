using Imya.Models.ModMetadata;
using Imya.Utils;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using Imya.Enums;
using Imya.Models.NotifyPropertyChanged;
using System;

namespace Imya.Models
{
    public class LocalizedText : PropertyChangedNotifier, IText
    {
        [JsonIgnore]
        public String Text
        {
            get { return _text; }
            private set
            {
                _text = value;
                OnPropertyChanged(nameof(Text));
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

        private void OnSerialized()
        {
            //Update(TextManager.Instance.ApplicationLanguage);
        }

        [OnDeserialized]
        private void OnSerialized(StreamingContext context)
        {
            OnSerialized();
        }

        public void Update(ApplicationLanguage lang)
        {
            switch (lang)
            {
                case ApplicationLanguage.English: if (English is String) Text = English; return;
                case ApplicationLanguage.German: if (German is String) Text = German; return;
                case ApplicationLanguage.Russian: if (Russian is String) Text = Russian; return;
                case ApplicationLanguage.Polish: if (Polish is String) Text = Polish; return;
            }
            Text = String.Empty;
        }

        public override string ToString()
        {
            return Text;
        }

    }
}
