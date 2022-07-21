using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.Attributes
{
    // TODO obsolete may be a different kind of state more similar to other compatibility issues.
    //      Obsolete may also be detected on startup, not only after zip installation.
    public enum ModStatus
    {
        Default,
        New,
        Updated,
        Obsolete
    }

    public class ModStatusAttributeFactory
    {
        private static Dictionary<ModStatus, IAttribute> static_attribs = new()
        {
            {
                ModStatus.Default,
                new GenericAttribute()
                {
                    Color = "Black",
                    Icon = "None",
                    AttributeType = AttributeType.ModStatus,
                    Description = new SimpleText("Mod Status Attribute")
                }
            },

            {
                ModStatus.New,
                new GenericAttribute()
                {
                    Color = "DodgerBlue",
                    Icon = "Download",
                    AttributeType = AttributeType.ModStatus,
                    Description = new SimpleText("Mod Status Attribute")
                }
            },
            {
                ModStatus.Updated,
                new GenericAttribute()
                {
                    Color = "LimeGreen",
                    Icon = "Update",
                    AttributeType = AttributeType.ModStatus,
                    Description = new SimpleText("Mod Status Attribute")
                }
            },
            {
                ModStatus.Obsolete,
                new GenericAttribute()
                {
                    Color = "Crimson",
                    Icon = "RemoveCircleOutline",
                    AttributeType = AttributeType.ModStatus,
                    Description = new SimpleText("Mod Status Attribute")
                }
            }
        };

        public static IAttribute Get(ModStatus status)
        {
            return static_attribs[status];
        }
    }
}
