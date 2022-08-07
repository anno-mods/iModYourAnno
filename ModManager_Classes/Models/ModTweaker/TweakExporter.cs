using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Imya.Models.ModTweaker
{
    public class TweakExporter
    {
        IEnumerable<ModOp> ModOps;
        IEnumerable<IExposedModValue> ExposedModValues;

        public TweakExporter(IEnumerable<ModOp> modops, IEnumerable<IExposedModValue> exposes) {
            ModOps = modops.Select(x => x.Clone()).ToList();
            ExposedModValues = exposes;
        }

        public IEnumerable<ModOp> GetExported() 
        {
            foreach (IExposedModValue val in ExposedModValues)
            { 
                ExecuteExpose(val);
            }
            foreach (ModOp op in ModOps)
            {
                if (op.Code.Count() == 0 && op.Type.Equals("replace"))
                {
                    op.Type = "remove";
                }
            }
            return ModOps;
        }

        /// <summary>
        /// Executes an Expose on the entire target document.
        /// </summary>
        /// <param name="expose"></param>
        /// <returns>Whether the expose needed to be executed.</returns>
        private void ExecuteExpose(IExposedModValue expose)
        {
            var ops = ModOps.Where(x => x.HasID && x.ID!.Equals(expose.ModOpID));

            foreach (ModOp n in ops)
            {
                List<XmlNode> newNodes = new List<XmlNode>();
                foreach (XmlNode x in n.Code)
                {
                    var edited_node = ExecuteExpose(x, expose);
                    if(edited_node is not null)
                        newNodes.Add(edited_node);
                }
                if (n.Code.Count() == 0 && !String.IsNullOrEmpty(expose.Value) && String.IsNullOrEmpty(expose.Path))
                {
                    var loadedNode = LoadXmlNodeFrom(expose.Value);
                    if(loadedNode is not null)
                        newNodes.Add(loadedNode);
                }
                n.Code = newNodes;
            }
        }

        private XmlNode? ExecuteExpose(XmlNode node, IExposedModValue expose)
        {
            var nodesToEdit = node.SelectNodes(expose.Path);
            if (nodesToEdit is null || nodesToEdit.Count == 0) return node;

            if (expose.ReplaceType == ExposedModValueReplaceType.Text)
            {
                foreach (XmlNode n in nodesToEdit) {
                    n.InnerText = expose.Value;
                }
            }
            else if (expose.ReplaceType == ExposedModValueReplaceType.Xml)
            {
                foreach (XmlNode n in nodesToEdit)
                {
                    var new_n = LoadXmlNodeFrom(expose.Value);

                    if (n == node)
                        return new_n;
                    else
                    {
                        n.ReplaceWith(new_n);
                    }
                }
            }
            //idk fuck the compiler
            return node;
        }

        private XmlNode? LoadXmlNodeFrom(String xml)
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.LoadXml(xml);
            }
            //ignore invalid xml or empty nodes
            catch (XmlException e)
            {
                return null;
            }
            return doc.FirstChild!;
        }
    }

    public static class XmlNodeExtensions
    {
        public static void ReplaceWith(this XmlNode self, XmlNode? other)
        {
            if (self.OwnerDocument is null) throw new ArgumentException("Cannot replace in a node wihtout owner document!");
            if (self.ParentNode is null)
            {
                var tmpParent = self.OwnerDocument.CreateElement("tempParent");
                tmpParent.AppendChild(self);
            }
            if (other is null)
            {
                self.ParentNode!.RemoveChild(self);
                return;
            }

            var imported_other = self.OwnerDocument.ImportNode(other, true);
            self.ParentNode!.ReplaceChild(imported_other, self);
        }
    }
}
