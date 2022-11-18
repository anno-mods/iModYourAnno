using Imya.Models.NotifyPropertyChanged;
using Imya.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Imya.Models.ModTweaker
{
    public struct TweakerConstants
    {
        public static readonly String EXPOSE_STRING = "ImyaExpose";
        public static readonly String EXPOSE_ATTR = "ExposeID";
        public static readonly String EXPOSE_PATH = "Path";
        public static readonly String MODOP_ID = "ModOpID";
        public static readonly String KIND = "Kind";
        public static readonly String DESCRIPTION = "Description";
        public static readonly String TOOLTIP = "Tooltip";
        public static readonly String ENUM_HEADER = "FixedValues";
        public static readonly String ENUM_ENTRY = "Value";
        public static readonly String ALT_TOGGLE_VAL = "AltValue";
        public static readonly String INVERTED = "Invert";

        public static readonly String PATH = "Path";
        public static readonly String TYPE = "Type";
        public static readonly String GUID = "GUID";
    }

    /// <summary>
    /// A collection of Exposed Mod Values in a File.
    /// 
    /// Can apply the exposes, adjusts the OG document to skip affected modops and generates the edited modop.s
    /// </summary>    
    public class TweakerFile : PropertyChangedNotifier
    {
        public String BasePath { get; private set; }
        public String FilePath { get; private set; }
        public String BaseFilePath => Path.GetDirectoryName(FilePath);
        public String SourceFilename { get; private set; }
        public String EditFilename => Path.GetFileNameWithoutExtension(SourceFilename) + ".imyatweak.include.xml";

        public string Title { get; private set; }

        public ObservableCollection<IExposedModValue> Exposes
        {
            get => _exposes;
            private set
            {
                _exposes = value;
                OnPropertyChanged(nameof(Exposes));
            }
        }
        private ObservableCollection<IExposedModValue> _exposes;

        //ModOps in here need to have an ID. This is considered by the XMLPatchParser.
        public ObservableCollection<ModOp> ModOps
        {
            get => _mod_ops;
            set
            {
                _mod_ops = value;
                OnPropertyChanged(nameof(ModOps));
            }
        }
        private ObservableCollection<ModOp> _mod_ops;

        private XmlDocument OriginalDocument { get; set; } = new XmlDocument();
        private XmlDocument TargetDocument { get; set; } = new XmlDocument();
        private XmlNode TargetRoot { get; set; }

        public ITweakStorage TweakStorage;

        private TweakerFile(String _filepath, String _basepath)
        {
            FilePath = _filepath;
            Title = Path.GetFileName(FilePath);
            BasePath = _basepath;

            SourceFilename = Path.GetFileName(_filepath);
        }

        private void InitTargetDocument()
        {
            TargetDocument = new XmlDocument();
            var node = TargetDocument.CreateElement("ModOps");
            TargetDocument.AppendChild(node);
            TargetRoot = node;
        }

        public IExposedModValue? GetExpose(String ExposeID)
        {
            return Exposes.First(x => x.ExposeID.Equals(ExposeID));
        }

        public bool ExposeExists(String ExposeID)
        {
            return Exposes.Any(x => x.ExposeID.Equals(ExposeID));
        }

        #region ModopManipulation

        /// <summary>
        /// Get's the default value for an expose from the original document
        /// </summary>
        /// <param name="expose"></param>
        /// <returns>The default value as String</returns>
        public String GetDefaultNodeValue(IExposedModValue expose)
        {
            var op = ModOps.Where(x => x.HasID && x.ID!.Equals(expose.ModOpID)).FirstOrDefault();
            if (op is null) return String.Empty;
            foreach (XmlNode x in op.Code)
            {
                var node = x.SelectSingleNode(expose.Path);
                if (node is not null) return node.InnerText;
            }
            return String.Empty;
        }

        /// <summary>
        /// Generates the ModOp as XML node in the target document
        /// </summary>
        /// <param name="modop"></param>
        /// <returns>The xml node that was generated</returns>
        public XmlNode? Generate(ModOp modop)
        {
            var elem = TargetDocument.CreateElement("ModOp");

            if (modop.Path is not null)
            {
                var PathAttr = TargetDocument.CreateAttribute(TweakerConstants.PATH);
                PathAttr.Value = modop.Path;
                elem.Attributes.Append(PathAttr);
            }

            if (modop.GUID is not null)
            {
                var GuidAttr = TargetDocument.CreateAttribute(TweakerConstants.GUID);
                GuidAttr.Value = modop.GUID;
                elem.Attributes.Append(GuidAttr);
            }

            var TypeAttr = TargetDocument.CreateAttribute(TweakerConstants.TYPE);
            TypeAttr.Value = modop.Type;

            elem.Attributes.Append(TypeAttr);

            foreach (XmlNode n in modop.Code)
            {
                //check whether we need deep cloning later
                var import = TargetDocument.ImportNode(n, true);
                elem.AppendChild(import);
            }

            TargetRoot.AppendChild(elem);

            return elem;
        }

        /// <summary>
        /// Ensures that an XML node has a skip attribute
        /// </summary>
        /// <param name="n"></param>
        public void EnsureSkipFlag(XmlNode? n)
        {
            if (n is null || n.OwnerDocument is null || n.TryGetAttribute("Skip", out var _)) return;

            var attrib = n.OwnerDocument!.CreateAttribute("Skip");
            attrib.Value = "1";
            n.Attributes!.Append(attrib);
        }

        public void EnsureInclude()
        {
            String Filepath = $"./{EditFilename}";
            if (OriginalDocument.SelectSingleNode($"/ModOps/Include[@File = '{Filepath}']") is not null) return;

            var n = OriginalDocument.CreateElement("Include");

            var attrib = OriginalDocument.CreateAttribute("File");
            attrib.Value = Filepath;
            n.Attributes!.Append(attrib);

            OriginalDocument.FirstChild?.AppendChild(n);
        }

        /// <summary>
        /// Executes an Expose on <paramref name="node"/>.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="expose"></param>
        /// <param name="NewValue"></param>
        /// <returns>Whether the expose needed to be executed.</returns>
        

        #endregion

        #region LoadSave

        public void Export()
        {
            TweakExporter exporter = new TweakExporter(ModOps, Exposes);
            var modops = exporter.GetExported();

            //ensure that the source document includes what we want.
            EnsureInclude();
            foreach (ModOp op in modops)
            {
                //ensure a skip in the source document.
                if (OriginalDocument.TryGetModOpNodes(op.ID!, out var modop_nodes))
                {
                    foreach (XmlNode node in modop_nodes!)
                        EnsureSkipFlag(node);
                }

                //generate the mod op in the target document
                Generate(op);
            }
        }

        public void Save(string basePath)
        {
            InitTargetDocument();
            Export();

            try
            {
                OriginalDocument.Save(Path.Combine(basePath, BaseFilePath, SourceFilename));
                TargetDocument.Save(Path.Combine(basePath, BaseFilePath, EditFilename));
            }
            catch (IOException e)
            {
                Console.WriteLine($"Failed to save Document {FilePath}. Cause: {e.Message}");
                return;
            }
        }

        public static bool TryInit(string basePath, string filePath, ITweakStorage tweakStorage, out TweakerFile tweakerFile)
        {
            tweakerFile = new(filePath, basePath);

            if (tweakerFile.TryLoadOwnerDocument(basePath, out var doc))
            {
                tweakerFile.OriginalDocument = doc;
                tweakerFile.BasePath = Path.GetFileName(basePath);
                
                tweakerFile.TweakStorage = tweakStorage;

                var parser = new XmlPatchParser(doc);
                var editables = parser.FetchExposes(tweakerFile);
                tweakerFile.Title = parser.FetchTweakerFileSettings() ?? tweakerFile.Title;

                tweakerFile.ModOps = new ObservableCollection<ModOp>(parser.FetchModOps(tweakerFile.TargetDocument));

                if (!editables.Any())
                {
                    return false;
                }
                tweakerFile.Exposes = new ObservableCollection<IExposedModValue>(editables);


                return true;
            }
            else
            {
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
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }

        #endregion
    }
}
