using System.Text.RegularExpressions;
using System.Xml;

namespace Imya.Models.ModTweaker
{
    public class ExposedModValue
    {
        public String Key { get; }
        private XmlNode ValueNode { get; }
        public String Value { get => ValueNode.InnerText; set => SetValue(value); }

        public ExposedModValue(XmlNode value, String key)
        {
            ValueNode = value; 
            Key = key;

            SetValue(ValueNode.InnerText);
        }

        public void SetValue(String content)
        {
            //ValueNode.Value = Regex.Replace(content, @"\s+", "");
            ValueNode.Value = content.TrimStart().TrimEnd();
        }
    }
}
