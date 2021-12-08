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

        
        #region ApplicationLanguage_Event

        

        #endregion

        public LanguageManager()
        {
            Instance = Instance ?? this; 
        }

        
    }
}
