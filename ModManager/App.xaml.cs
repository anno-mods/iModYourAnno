using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ModManager_Classes.src.Handlers;
using ModManager_Classes.src.Enums;

namespace ModManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            //Setup Managers
            LanguageManager LanguageManager = new LanguageManager();
            TextManager TextManager = new TextManager("texts.json");
            LanguageManager.Instance.ChangeLanguage(ApplicationLanguage.English);
        }
    }
}
