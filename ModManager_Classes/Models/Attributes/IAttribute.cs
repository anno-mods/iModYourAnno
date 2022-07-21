using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.Attributes
{
    public enum AttributeType { ModStatus }

    public interface IAttribute
    {
        AttributeType AttributeType { get; }
        IText Description { get; }
        String Icon { get; }
        String Color { get; }
    }
}
