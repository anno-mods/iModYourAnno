using Imya.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Imya.Models.ModTweaker
{
    internal class XmlPatchParser
    {
        private XmlDocument Document;

        internal XmlPatchParser(XmlDocument doc)
        {
            Document = doc;
        }

        /*
        internal IEnumerable<ExposedModValue> FetchExposedValues(TweakerFile parent)
        {
            var ExposedValues = Document.SelectNodes($"//{TweakerConstants.EXPOSE_STRING}");
            if (ExposedValues is null) yield break;

            foreach (XmlNode ExposeInstruction in ExposedValues)
            {
                //per expose instruction
                Expose? expose = Expose.FromXmlNode(ExposeInstruction);

                if (expose is not null && TryFetchExposedValue(expose, parent, out var ExposedModValue))
                {
                    yield return ExposedModValue;
                }
            }
        }
        */

        internal IEnumerable<ExposedModValue> FetchExposes(TweakerFile parent)
        {
            var ExposedValues = Document.SelectNodes($"//{TweakerConstants.EXPOSE_STRING}");
            if (ExposedValues is null) yield break;

            foreach (XmlNode ExposeInstruction in ExposedValues)
            {
                //per expose instruction
                ExposedModValue? expose = ExposedModValue.FromXmlNode(ExposeInstruction);

                expose.Parent = parent;

                if (parent.TweakStorage.TryGetTweakValue(parent.FilePath, expose.ExposeID, out var value))
                {
                    expose.Value = value!;
                }
                else
                {
                    expose.Value = parent.GetDefaultNodeValue(expose);
                }

                if (expose is not null)
                {
                    yield return expose;
                }
            }
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
        /*
        internal bool TryFetchExposedValue(Expose Expose, TweakerFile parent, out ExposedModValue exposedModValue)
        {
            if (Expose.Path is not null && Expose.ExposeID is not null && Document.TryGetModOpNode("Heater", out var modop))
            {
                var node = modop.SelectSingleNode(Expose.Path);
                var TextNode = node?.SelectSingleNode("./text()");

                if (TextNode is not null)
                {
                    exposedModValue = new ExposedModValue(TextNode, Expose.ExposeID, parent);
                    return true;
                }
            }
            exposedModValue = null;
            return false;
        }*/
    }
}
