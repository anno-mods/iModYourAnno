﻿using Imya.Utils;
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
            get => GetShortcut(nameof(Skip), acceptNull: true);
            set => SetShortcut(nameof(Skip), value, acceptNull: true);
        }

        //Mod Op related things
        public string Type 
        {
            get => GetShortcut(nameof(Type));
            set => SetShortcut(nameof(Type), value);
        }

        public string? GUID
        { 
            get => GetShortcut(nameof(GUID), acceptNull: true);
            set => SetShortcut(nameof(GUID), value, acceptNull: true);
        }

        public string? Path
        {
            get => GetShortcut(nameof(Path), acceptNull: true);
            set => SetShortcut(nameof(Path), value, acceptNull: true);
        }

        public Dictionary<string, string?> Attributes { get; init; }

        public bool IsValid => GUID is string || Path is string;
        public bool HasID => ID is string;

        private string? GetShortcut(string attributeKey, bool acceptNull = false)
        {
            return Attributes.GetValueOrDefault(attributeKey)
                ?? (acceptNull ? null : throw new InvalidDataException($"ModOp without {attributeKey} attribute!"));
        }

        private void SetShortcut(string attributeKey, string? value, bool acceptNull = false)
        {
            if (!acceptNull && value is null)
            {
                throw new ArgumentException($"Cannot assign a null value to {attributeKey} attribute!");
            }
            if (Attributes.ContainsKey(attributeKey))
            {
                Attributes[attributeKey] = value;
                return; 
            }

            Attributes.Add(attributeKey, value);
        }

        public static ModOp? FromXmlNode(XmlNode ModOp)
        {
            if (ModOp.NodeType != XmlNodeType.Element)
                throw new ArgumentException();

            //hacky shortcuts for includes and groups
            string? type = null;
            if (ModOp.Name.ToLower() == "include")
                type = "include";
            else if (ModOp.Name.ToLower() == "group")
                type = "group";

            if (type is null && ModOp.TryGetAttribute(TweakerConstants.TYPE, out string? ModOpType))
                type = ModOpType;

            if (type is not null)
            {
                ModOp.TryGetAttribute(TweakerConstants.MODOP_ID, out string? ID);
                var dict = new Dictionary<string, string>();
                foreach (XmlAttribute attribute in ModOp.Attributes!)
                {
                    dict.TryAdd(attribute.Name, attribute.Value);
                }
                return new ModOp
                {
                    ID = ID!,
                    Code = ModOp.ChildNodes.Cast<XmlNode>().ToList(),
                    Attributes = dict,
                    Type = type
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
                Attributes = Attributes
            };
        }
    }

}
