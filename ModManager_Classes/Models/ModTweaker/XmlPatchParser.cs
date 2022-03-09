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
        private static readonly String EXPOSE_STRING = "ImyaExpose";
        private static readonly String EXPOSE_ATTR = "ExposeID";
        private static readonly String EXPOSE_PATH = "Path";

        private XmlDocument Document;

        internal XmlPatchParser(XmlDocument doc)
        {
            Document = doc;
        }

        internal IEnumerable<ExposedModValue> FetchExposedValues()
        {
            var ExposedValues = Document.SelectNodes($"//{EXPOSE_STRING}");

            foreach (XmlNode ExposeInstruction in ExposedValues)
            {
                //per expose instruction
                String? Path = ExposeInstruction.Attributes?[EXPOSE_PATH]?.Value;
                String? ExposeID = ExposeInstruction.Attributes?[EXPOSE_ATTR]?.Value;

                if (TryFetchExposedValue(Path, ExposeID, out var ExposedModValue))
                {
                    yield return ExposedModValue;
                }
            }
        }

        internal bool TryFetchExposedValue(String Path, String ExposeID, out ExposedModValue exposedModValue)
        {
            if (Path is not null && ExposeID is not null)
            {
                var node = Document.SelectSingleNode(Path);
                var TextNode = node?.SelectSingleNode("./text()");

                if (TextNode is not null)
                {
                    exposedModValue = new ExposedModValue(TextNode, ExposeID);
                    return true;
                }
            }
            exposedModValue = null;
            return false;
        }
    }
}
