using Imya.Models;
using Imya.Models.ModMetadata.ModinfoModel;
using Newtonsoft.Json;

// TODO move all text/language related code under Imya.Text or Imya.Language
namespace Imya.Texts
{
    public class TextManager : ITextManager
    {
        public ApplicationLanguage ApplicationLanguage { get; private set; } = ApplicationLanguage.English;

        private readonly Dictionary<string, IText> KeyedTexts = new();
        private readonly List<IText> UnkeyedTexts = new();

        public event ITextManager.LanguageChangedEventHandler LanguageChanged = delegate { };

        public IText this[String Key]
        {
            get { return GetText(Key); }
        }

        public TextManager()
        {

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
            catch (Exception e)
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

        public LocalizedText CreateLocalizedText(Localized? localized, string? defaultText)
        {
            string? english = localized?.English ?? defaultText;

            var newText = new LocalizedText
            {
                Chinese = localized?.Chinese ?? english,
                English = english,
                French = localized?.French ?? english,
                German = localized?.German ?? english,
                Italian = localized?.Italian ?? english,
                Japanese = localized?.Japanese ?? english,
                Korean = localized?.Korean ?? english,
                Polish = localized?.Polish ?? english,
                Russian = localized?.Russian ?? english,
                Spanish = localized?.Spanish ?? english,
                Taiwanese = localized?.Taiwanese ?? english
            };

            newText.Update(ApplicationLanguage);
            AddAnonymousText(newText);

            return newText;
        }
    }
}
