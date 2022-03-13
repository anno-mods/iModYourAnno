using Imya.Models.NotifyPropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Imya.Models.ModTweaker
{
    public class TweakerFile : PropertyChangedNotifier
    {
        public string FilePath { get; private set; }
        public ObservableCollection<ExposedModValue> EditableValues 
        {
            get => _editableValues;
            private set
            {
                _editableValues = value;
                OnPropertyChanged(nameof(EditableValues));
            }
        }
        private ObservableCollection<ExposedModValue> _editableValues;
        public XmlDocument OwnerDocument { get; private set; } = new XmlDocument();

        private FileSystemWatcher watcher;

        private TweakerFile()
        {

        }

        public void Save(string basePath)
        {
            try
            {
                OwnerDocument.Save(Path.Combine(basePath, FilePath));
            }
            catch (IOException e)
            {
                Console.WriteLine($"Failed to save Document {FilePath}. Cause: {e.Message}");
            }
        }

        public static bool TryInit(string basePath, string filePath, out TweakerFile tweakerFile)
        {
            tweakerFile = new();
            tweakerFile.FilePath = filePath;

            if (tweakerFile.TryLoadOwnerDocument(basePath, out var doc))
            {
                tweakerFile.OwnerDocument = doc;

                var parser = new XmlPatchParser(doc);
                var editables = parser.FetchExposedValues();
                if (!editables.Any())
                {
                    return false;
                }

                tweakerFile.EditableValues = new ObservableCollection<ExposedModValue>(editables);
                return true;
            }
            else {
                Console.WriteLine($"Could not load Document: {tweakerFile.FilePath}");
            }
            return false;
        }

        private bool TryLoadOwnerDocument(string basePath, out XmlDocument doc)
        {
            doc = new XmlDocument();
            try
            {
                doc.Load(Path.Combine(basePath, FilePath));
            }
            catch (XmlException e)
            {
                return false;
            }
            return true;
        }
    }
}
