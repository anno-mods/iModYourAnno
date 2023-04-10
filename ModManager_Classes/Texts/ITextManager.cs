using Imya.Models;
using Imya.Models.ModMetadata;
using Imya.Models.ModMetadata.ModinfoModel;
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

        IText this[String Key] { get; }

        void AddText(String Key, IText t);
        void AddAnonymousText(IText t);
        IText GetText(String Key);
        void ChangeLanguage(ApplicationLanguage lang);
        void UpdateTexts();
        void LoadLanguageFile(String Sourcefile);
        LocalizedText CreateLocalizedText(Localized localized);
    }
}
