using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.Attributes
{
    public interface IAttributeCollection : IEnumerable<IAttribute>
    {
        public void AddAttribute(IAttribute attrib);
        public void RemoveAttribute(IAttribute attrib);
        public void RemoveAttributesByType(AttributeType type);

        public IAttribute? GetByType(AttributeType type);
        public bool HasAttribute(AttributeType type);
    }
}
