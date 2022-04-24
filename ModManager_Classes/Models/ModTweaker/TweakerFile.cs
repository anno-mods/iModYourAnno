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

        public static readonly String PATH = "Path";
        public static readonly String TYPE = "Type";
        public static readonly String GUID = "GUID";
    }
    
    /// <summary>
    /// This is basically the same as ExposedModValue
    /// </summary>
    
    public class TweakerFile : PropertyChangedNotifier
    {
        public String BasePath { get; private set; }
        public String FilePath { get; private set; }
        public String BaseFilePath => Path.GetDirectoryName(FilePath);
        public String SourceFilename { get; private set; }
        public String EditFilename => Path.GetFileNameWithoutExtension(SourceFilename) + ".imyatweak.xml";

        public ObservableCollection<ExposedModValue> Exposes
        {
            get => _exposes;
            private set
            {
                _exposes = value;
                OnPropertyChanged(nameof(Exposes));
            }
        }
        private ObservableCollection<ExposedModValue> _exposes;

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
        private XmlNode TargetRoot { get; init; }

        public ITweakStorage TweakStorage;

        private TweakerFile(String _filepath, String _basepath)
        {
            FilePath = _filepath;
            BasePath = _basepath;

            SourceFilename = Path.GetFileName(_filepath);

            var node = TargetDocument.CreateElement("ModOps");
            TargetDocument.AppendChild(node);
            TargetRoot = node;
        }

        public ExposedModValue? GetExpose(String ExposeID)
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
        public String GetDefaultNodeValue(ExposedModValue expose)
        {
            var op = ModOps.Where(x => x.HasID && x.ID!.Equals(expose.ModOpID)).First();

            foreach (XmlNode x in op.Code)
            {
                var node = OriginalDocument.SelectSingleNode(expose.Path);
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
        /// Executes A collection of exposes on <paramref name="node"/>. 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="exposes"></param>
        /// <param name="NewValue"></param>
        /// <returns>Whether any exposes were executed on the node.</returns>
        public bool ExecuteExposes(XmlNode node, IEnumerable<ExposedModValue> exposes, String NewValue)
        {
            bool AnyExecutes = false;
            foreach (ExposedModValue x in exposes)
            {
                bool Executed = ExecuteExpose(node, x);
                //if no executes were done yet, consider the current value, else stay true.
                AnyExecutes = AnyExecutes ? true : Executed;
            }
            return AnyExecutes;
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
            String Filepath = $"../{EditFilename}";
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
        private bool ExecuteExpose(XmlNode node, ExposedModValue expose)
        {
            var nodesToEdit = node.SelectNodes(expose.Path);
            if (nodesToEdit is null || nodesToEdit.Count == 0) return false;

            foreach (XmlNode n in nodesToEdit)
            {
                n.InnerText = expose.Value;
            }
            return true;
        }

        /// <summary>
        /// Executes an Expose on the entire target document.
        /// </summary>
        /// <param name="expose"></param>
        /// <returns>Whether the expose needed to be executed.</returns>
        internal bool ExecuteExpose(ExposedModValue expose)
        {
            var ops = ModOps.Where(x => x.HasID && x.ID!.Equals(expose.ModOpID));

            List<bool> SuccessTracker = new();
            foreach (ModOp n in ops)
            {
                foreach (XmlNode x in n.Code)
                {
                    bool b = ExecuteExpose(x, expose);
                    SuccessTracker.Add(b);
                }
            }
            return SuccessTracker.Any(x => x == true);
        }

        #endregion

        #region LoadSave

        public void Export()
        {
            foreach (ExposedModValue e in Exposes)
            {
                ExecuteExpose(e);
            }
            //ensure that the source document includes what we want.
            EnsureInclude();
            foreach (ModOp op in ModOps)
            {
                //ensure a skip in the source document.
                OriginalDocument.TryGetModOpNode(op.ID!, out var modop);
                EnsureSkipFlag(modop);

                //generate the mod op in the target document
                Generate(op);
            }
        }

        public void Save(string basePath)
        {
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

                tweakerFile.ModOps = new ObservableCollection<ModOp>(parser.FetchModOps(tweakerFile.TargetDocument));

                if (!editables.Any())
                {
                    return false;
                }
                tweakerFile.Exposes = new ObservableCollection<ExposedModValue>(editables);


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
