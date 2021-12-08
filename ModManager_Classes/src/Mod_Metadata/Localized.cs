using System;
using System.Collections.Generic;
using System.Text;
using ModManager_Classes.src.Enums;
using ModManager_Classes.src.Handlers;

namespace ModManager_Classes.src.Models
{
    public class Localized
    {
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

        public Localized() { }

        public Localized(String s)
        {
            English = s;
            German = s;
            Chinese = s;
            French = s;
            Italian = s;
            Japanese = s;
            Korean = s;
            Polish = s;
            Russian = s;
            Spanish = s;
            Taiwanese = s;
        }

        public String getText(ApplicationLanguage appLang)
        {
            switch (appLang)
            {
                case ApplicationLanguage.English: if (English is String) return English; break;
                case ApplicationLanguage.German: if (German is String) return German; break;
                default: if (English is String) return English; break;
            }
            return String.Empty;
        }

        public String getText()
        {
            return getText(TextManager.Instance.ApplicationLanguage);
        }
    }
}
