using Anno.EasyMod.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.Attributes
{
    public class GenericAttribute : IModAttribute
    {
        public string AttributeType { get; init; }
        public IText Description { get; init; }

        bool IModAttribute.MultipleAllowed => false;
    }
}
