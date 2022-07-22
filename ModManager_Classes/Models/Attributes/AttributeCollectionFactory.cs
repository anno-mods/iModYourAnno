using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.Attributes
{
    public class AttributeCollectionFactory
    {
        public static Type? AttributeCollectionType {
            get => _attributeCollectionType;
            set
            {
                if (!typeof(IAttributeCollection).IsAssignableFrom(value)) throw new InvalidOperationException();
                _attributeCollectionType = value;
            }
        }
        private static Type? _attributeCollectionType;

        public static IAttributeCollection GetNew()
        {
            return Activator.CreateInstance(AttributeCollectionType) as IAttributeCollection;
        }
    }
}
