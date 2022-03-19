using System.Runtime.Serialization;
using Imya.Models;
using Newtonsoft.Json;
using Imya.Enums;
using Imya.Models.ModMetadata;

// TODO move all text/language related code under Imya.Text or Imya.Language
namespace Imya.Utils
{
    public class TextManager
    {
        public static TextManager Instance { get; private set; }

        public ApplicationLanguage ApplicationLanguage { get; private set; } = ApplicationLanguage.English;

        private readonly Dictionary<string, IText> KeyedTexts = new();
        private readonly List<IText> UnkeyedTexts = new();

        public delegate void LanguageChangedEventHandler(ApplicationLanguage language);
        public event LanguageChangedEventHandler LanguageChanged = delegate { };

        public IText this[String Key]
        {
            get { return Instance.GetText(Key); }
        }

        public TextManager()
        {
            Instance ??= this;
        }

        public void AddText(String Key, IText t)
        {
            try
            {
                KeyedTexts.Add(Key, t);
                t.Update(ApplicationLanguage);
            }
            catch
            {
                Console.WriteLine($"Could not add Text \"{Key}\": Key already used");
            }
        }

        public void AddAnonymousText(IText Text)
        {
            UnkeyedTexts.Add(Text);
            Text.Update(ApplicationLanguage);
        }

        public IText GetText(String Key)
        {
            try
            {
                return KeyedTexts[Key];
            }
            catch
            {
                Console.WriteLine($"Could not find Text: {Key}");
                return IText.Empty;
            }
        }

        public void ChangeLanguage(ApplicationLanguage lang)
        {
            Console.WriteLine($"Changed App Language to: {lang}");
            ApplicationLanguage = lang;

            UpdateTexts();
            // notify all who may not have the possibility to rely on PropertyChanged
            LanguageChanged.Invoke(lang);
        }

        public void UpdateTexts()
        {
            foreach (var k in KeyedTexts)
            {
                k.Value.Update(ApplicationLanguage);
            }

            foreach (var text in UnkeyedTexts)
            {
                text.Update(ApplicationLanguage);
            }
        }

        public void LoadLanguageFile(String Sourcefile)
        {
            KeyedTexts.Clear();

            try
            {
                Dictionary<String, LocalizedText> localizedTexts = JsonConvert.DeserializeObject<Dictionary<String, LocalizedText>>(File.ReadAllText(Sourcefile)) ?? new Dictionary<string, LocalizedText>();
                foreach (var x in localizedTexts)
                {
                    AddText(x.Key, x.Value);
                }
            }
            catch (IOException e)
            {
                Console.WriteLine($"Error accessing file \"{Sourcefile}\": {e.Message}");
            }
            catch (JsonSerializationException e)
            {
                Console.WriteLine($"Error while parsing file \"{Sourcefile}\": {e.Message}");
            }

        }

        public static LocalizedText CreateLocalizedText(Localized localized)
        {
            var newText = new LocalizedText();

            if (localized.Chinese is String) newText.Chinese = localized.Chinese;
            if (localized.English is String) newText.English = localized.English;
            if (localized.French is String) newText.French = localized.French;
            if (localized.German is String) newText.German = localized.German;
            if (localized.Italian is String) newText.Italian = localized.Italian;
            if (localized.Japanese is String) newText.Japanese = localized.Japanese;
            if (localized.Korean is String) newText.Korean = localized.Korean;
            if (localized.Polish is String) newText.Polish = localized.Polish;
            if (localized.Russian is String) newText.Russian = localized.Russian;
            if (localized.Spanish is String) newText.Spanish = localized.Spanish;
            if (localized.Taiwanese is String) newText.Taiwanese = localized.Taiwanese;

            if (Instance is not null)
            {
                newText.Update(Instance.ApplicationLanguage);
                Instance.AddAnonymousText(newText);
            }
            return newText;
        }
    }
}
