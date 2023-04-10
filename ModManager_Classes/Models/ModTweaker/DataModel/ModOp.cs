using Imya.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Imya.Models.ModTweaker.DataModel
{
    public class ModOp
    {
        public string? ID;
        public IEnumerable<XmlNode> Code;
        public string? Skip;

        //Mod Op related things
        public string Type;
        public string? GUID;
        public string? Path;

        public bool IsValid => GUID is string || Path is string;
        public bool HasID => ID is string;

        public static ModOp? FromXmlNode(XmlNode ModOp)
        {
            string? type = null;
            if (ModOp.Name.ToLower() == "include")
                type = "include";
            if (type is null && ModOp.TryGetAttribute(TweakerConstants.TYPE, out string? ModOpType))
                type = ModOpType;

            if (type is not null)
            {
                ModOp.TryGetAttribute(TweakerConstants.GUID, out string? Guid);
                ModOp.TryGetAttribute(TweakerConstants.PATH, out string? Path);
                ModOp.TryGetAttribute(TweakerConstants.MODOP_ID, out string? ID);
                return new ModOp
                {
                    ID = ID!,
                    Code = ModOp.ChildNodes.Cast<XmlNode>().ToList(),
                    Type = type,
                    GUID = Guid,
                    Path = Path,
                    Skip = ModOp.Attributes?["Skip"]?.Value
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
                Type = Type,
                GUID = GUID,
                Path = Path
            };
        }
    }

}
