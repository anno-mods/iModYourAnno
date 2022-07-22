using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.Attributes
{
    public class TweakedAttributeFactory
    {
        static GenericAttribute TweakedAttribute = 
            new GenericAttribute()
            {
                AttributeType = AttributeType.TweakedMod,
                Description = new SimpleText("The Mod has been tweaked")
            };

        public static IAttribute Get()
        {
            return TweakedAttribute;
        }
    }
}
