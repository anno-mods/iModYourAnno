using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModManager_Classes.src.Enums;
using ModManager_Classes.src.Models;

namespace ModManager_Classes.src.Handlers
{
    public class LanguageManager
    {
        public static LanguageManager Instance { get; private set; }

        public ApplicationLanguage ApplicationLanguage { get; private set; }

        #region ApplicationLanguage_Event

        public delegate void LanguageChangedEventHandler (ApplicationLanguage language);
        public event LanguageChangedEventHandler LanguageChanged = delegate { };

        #endregion

        public LanguageManager()
        {
            Instance = Instance ?? this; 
        }

        public void ChangeLanguage(ApplicationLanguage lang)
        {
            Console.WriteLine($"Changed App Language to: {lang}");
            ApplicationLanguage = lang;
            LanguageChanged(lang);
        }
    }
}
