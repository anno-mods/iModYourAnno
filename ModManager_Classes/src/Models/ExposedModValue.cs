using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Collections.ObjectModel;

namespace ModManager_Classes.src.Models
{
    public class ExposedModValue
    {
        private String RelativePath; 
        String AbsolutePath { get => RelativePath; }
        String Key { get; }
        XmlNode Value { get; }

        public ExposedModValue(String relativePath, XmlNode value, String key)
        {
            RelativePath = relativePath;
            Value = value; 
            Key = key;
        }

        public void Set(String content)
        { 
            Value.InnerText = content;
        }

        public void Save()
        {
            if (Value.OwnerDocument is XmlDocument)
            {
                Value.OwnerDocument.Save(AbsolutePath);
            }
            else
            {
                Console.WriteLine($"Failed to Save \"{AbsolutePath}\". Value Owner Document does not exist!");
            }
        }
    }
}
