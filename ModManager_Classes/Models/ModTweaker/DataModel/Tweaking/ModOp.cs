using Imya.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Imya.Models.ModTweaker.DataModel.Tweaking
{
    public class ModOp
    {
        public string? ID;
        public IEnumerable<XmlNode> Code;


        public string? Skip
        {
            get => GetShortcut(nameof(Skip));
            set => SetShortcut(nameof(Skip), value);
        }

        //Mod Op related things
        public string Type 
        {
            get => GetShortcut(nameof(Type));
            set => SetShortcut(nameof(Type), value);
        }

        public string? GUID
        { 
            get => GetShortcut(nameof(GUID));
            set => SetShortcut(nameof(GUID), value);
        }

        public string? Path
        {
            get => GetShortcut(nameof(Path));
            set => SetShortcut(nameof(Path), value);
        }

        public XmlAttributeCollection XmlAttributes { get; init; }

        public bool IsValid => GUID is string || Path is string;
        public bool HasID => ID is string;

        private string? GetShortcut(string attributeKey)
        {
            return XmlAttributes.GetNamedItem(attributeKey)?.Value
                ?? throw new InvalidDataException($"ModOp without {attributeKey} attribute!");
        }

        private void SetShortcut(string attributeKey, string? value)
        {
            try
            {
                XmlAttributes.GetNamedItem(attributeKey)!.Value ??= value;
            }
            catch (NullReferenceException e)
            {
                throw new InvalidDataException($"ModOp without {attributeKey} attribute!");
            }
        }

        public static ModOp? FromXmlNode(XmlNode ModOp)
        {
            if (ModOp.NodeType != XmlNodeType.Element)
                throw new ArgumentException();

            string? type = null;
            if (ModOp.Name.ToLower() == "include")
                type = "include";
            if (type is null && ModOp.TryGetAttribute(TweakerConstants.TYPE, out string? ModOpType))
                type = ModOpType;

            if (type is not null)
            {
                ModOp.TryGetAttribute(TweakerConstants.MODOP_ID, out string? ID);
                return new ModOp
                {
                    ID = ID!,
                    Code = ModOp.ChildNodes.Cast<XmlNode>().ToList(),
                    XmlAttributes = ModOp.Attributes!
                };
            }
            return null;
        }

        public ModOp Clone()
        {
            return new ModOp()
            {
                ID = ID,
                Code = Code.Select(x => x.CloneNode(true)).ToList(),
                XmlAttributes = XmlAttributes
            };
        }
    }

}
