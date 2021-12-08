using Imya.Models.ModMetadata;
using Imya.Utils;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Imya.Models
{
    public class LocalizedText: INotifyPropertyChanged
    {
        [JsonIgnore]
        public String Text {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged("Text");
            }
        }
        [JsonIgnore]
        private String _text;

        public Localized? Texts { get; set; }
        
        public LocalizedText()
        {

        }

        public LocalizedText(Localized l)
        {
            Texts = l;
            OnSerialized(); 
        }

        private void OnSerialized()
        {
            if (Texts is Localized)
            {
                Text = Texts.getText();
            }
            else
            {
                Text = String.Empty;
            }
            TextManager.Instance.LanguageChanged += OnLanguageChanged;
        }

        [OnDeserialized]
        private void OnSerialized(StreamingContext context)
        {
            OnSerialized();
        }

        public void OnLanguageChanged(ApplicationLanguage lang)
        {
            if (Texts is Localized)
            {
                Text = Texts.getText(lang);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged = delegate { };

        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler is PropertyChangedEventHandler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public override string ToString()
        {
            return Text;
        }

    }
}
