using ModManager_Classes.src.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace ModManager_Classes.src.Handlers
{
    public class TextManager
    {
        public static TextManager Instance;
        private Dictionary<string, LocalizedText> Texts;

        public TextManager(String Sourcefile)
        {
            Instance = Instance ?? this;
            try
            {
                Texts = JsonConvert.DeserializeObject<Dictionary<String, LocalizedText>>(File.ReadAllText(Sourcefile)) ?? new Dictionary<string, LocalizedText>();
            }
            catch
            {
                Texts = new Dictionary<string, LocalizedText>();
                Console.WriteLine($"Error loading Text file: {Sourcefile}");
            }
        }

        public void AddText(String Key, LocalizedText t)
        {
            try
            {
                Texts.Add(Key, t);
            }
            catch
            {
                Console.WriteLine($"Could not add Text: {Key}");
            }
        }

        public LocalizedText GetText(String Key)
        {
            try
            {
                return Texts[Key];
            }
            catch
            {
                Console.WriteLine($"Could not find Text: {Key}");
                return new LocalizedText();
            }
        }

        public void SetVariable(ref String variable, String Key)
        {
            variable = this.GetText(Key).Text;
        }
    }
}
