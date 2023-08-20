using Anno.EasyMod.Metadata;
using Imya.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Texts
{
    public interface ITextManager
    {
        ApplicationLanguage ApplicationLanguage { get; }

        delegate void LanguageChangedEventHandler(ApplicationLanguage language);
        event LanguageChangedEventHandler LanguageChanged;

        public IText this[String Key] { get; }

        void AddText(String Key, IText t);
        void AddAnonymousText(IText t);
        IText GetText(String Key);
        void ChangeLanguage(ApplicationLanguage lang);
        void UpdateTexts();
        void LoadLanguageFile(String Sourcefile);
        LocalizedText CreateLocalizedText(Localized? localized, string? defaultText);
    }
}
