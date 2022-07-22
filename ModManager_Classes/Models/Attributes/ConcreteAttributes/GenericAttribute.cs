using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.Attributes
{
    public class GenericAttribute : IAttribute
    {
        public AttributeType AttributeType { get; init; }
        public IText Description { get; init; }
        public string Icon { get; init; }
        public String Color { get; init; }
    }
}
