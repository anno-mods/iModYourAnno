using Imya.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Imya.Models.ModTweaker
{
    /// <summary>
    /// Object that scans XML documents for Imya Expose Instructions and ModOps that are affected by it.
    /// </summary>
    internal class XmlPatchParser
    {
        private XmlDocument Document;

        internal XmlPatchParser(XmlDocument doc)
        {
            Document = doc;
        }

        internal IEnumerable<IExposedModValue> FetchExposes(TweakerFile parent)
        {
            var ExposedValues = Document.SelectNodes($"//{TweakerConstants.EXPOSE_STRING}");
            if (ExposedValues is null) yield break;

            foreach (XmlNode ExposeInstruction in ExposedValues)
            {
                //per expose instruction
                IExposedModValue? expose = FetchExpose(ExposeInstruction, parent);
                if (expose is not null)
                {
                    yield return expose;
                }
            }
        }

        private IExposedModValue? FetchExpose(XmlNode ExposeInstruction, TweakerFile parent)
        {
            //per expose instruction
            IExposedModValue? expose = ExposedModValueFactory.FromXmlNode(ExposeInstruction, parent);

            if (expose is null) return null;

            if (parent.TweakStorage.TryGetTweakValue(parent.FilePath, expose.ExposeID, out var value))
            {
                expose.Value = value!;
            }
            else
            {
                expose.Value = parent.GetDefaultNodeValue(expose);
            }

            return expose;
        }

        internal IEnumerable<ModOp> FetchModOps(XmlDocument ImportDocument)
        { 
            var ModOps = Document.SelectNodes("/ModOps/ModOp");

            foreach (XmlNode _ModOp in ModOps)
            {
                var Imported = ImportDocument.ImportNode(_ModOp, true);
                ModOp? op = ModOp.FromXmlNode(Imported);
                if (op is not null && op.HasID) yield return op;
            }
        }

        public string? FetchTweakerFileSettings()
        {
            var tweaks = Document.SelectSingleNode("/ModOps/ImyaTweaks");
            if (tweaks is null)
                return null;

            return tweaks.Attributes?["Title"]?.Value;
        }
    }
}
