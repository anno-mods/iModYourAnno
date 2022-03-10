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
        public String Filename { get; private set; }
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

        public void Save()
        {
            try
            {
                OwnerDocument.Save(Filename);
            }
            catch (IOException e)
            {
                Console.WriteLine($"Failed to save Document {Filename}. Cause: {e.Message}");
            }
        }

        public static bool TryInit(String Filename, out TweakerFile tweakerFile)
        {
            tweakerFile = new TweakerFile();
            tweakerFile.setFilename(Filename);

            if (tweakerFile.TryLoadOwnerDocument(out var doc))
            {
                tweakerFile.setOwnerDocument(doc);


                XmlPatchParser parser = new XmlPatchParser(doc);
                var Editables = parser.FetchExposedValues();

                if (Editables.Count() <= 0)
                {
                    return false;
                }

                tweakerFile.setEditableValues(Editables);
                return true;
            }
            else {
                Console.WriteLine($"Could not load Document: {Filename}");
            }
            return false;
        }

        private bool TryLoadOwnerDocument(out XmlDocument doc)
        {
            doc = new XmlDocument();
            try
            {
                doc.Load(Filename);
            }
            catch (XmlException e)
            {
                return false;
            }
            return true;
        }

        private void setOwnerDocument(XmlDocument doc)
        {
            OwnerDocument = doc;
        }

        private void setEditableValues(IEnumerable<ExposedModValue> values)
        {
            EditableValues = new ObservableCollection<ExposedModValue>(values);
        }

        private void setFilename(String s)
        {
            Filename = s;
        }
    }
}
