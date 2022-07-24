using Imya.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.Attributes
{
    public class MissingModinfoAttributeFactory
    {
        static GenericAttribute MissingModinfoAttribute =
           new GenericAttribute()
           {
               AttributeType = AttributeType.MissingModinfo,
               Description = TextManager.Instance.GetText("ATTRIBUTE_NOMODINFO")
           };

        public static IAttribute Get()
        {
            return MissingModinfoAttribute;
        }
    }
}
