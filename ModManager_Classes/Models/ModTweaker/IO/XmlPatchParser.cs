using Imya.Models.ModTweaker.DataModel.Tweaking;
using Imya.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Imya.Models.ModTweaker.IO
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
            expose.Value = parent.GetDefaultNodeValue(expose);
            return expose;
        }

        internal IEnumerable<ModOp> FetchModOps(XmlDocument ImportDocument)
        {
            var modOps = Document.SelectNodes("/ModOps/ModOp | /ModOps/Include");
            if (modOps is null)
                yield break;

            foreach (XmlNode modOp in modOps)
            {
                var imported = ImportDocument.ImportNode(modOp, true);
                ModOp? op = ModOp.FromXmlNode(imported);
                if (op is not null && op.HasID)
                    yield return op;
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
