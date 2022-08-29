using Imya.Models.Attributes;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.UnitTests.Models
{
    internal class TestAttributeCollection : IAttributeCollection
    {
        // TODO ObservableCollection in UI is not thread-safe, so why should this?...
        // TODO test should not rely on it's own collection implementation
        // TODO what actually is needed is a sorted list on attributetype with multiple values per key
        private readonly List<IAttribute>[] Attributes;

        public TestAttributeCollection()
        {
            var types = Enum.GetValues(typeof(AttributeType));
            Attributes = new List<IAttribute>[types.Length];
            for (int i = 0; i < types.Length; i++)
                Attributes[i] = new List<IAttribute>();
        }

        public void AddAttribute(IAttribute attrib)
        {
            //lock (this)
            {
                if (!attrib.MultipleAllowed && Attributes[(int)attrib.AttributeType].Count > 0)
                    return;

                Attributes[(int)attrib.AttributeType].Add(attrib);
            }
        }

        public IAttribute? GetByType(AttributeType type)
        {
            //lock (this)
                return Attributes[(int)type].FirstOrDefault();
        }

        public IEnumerator<IAttribute> GetEnumerator()
        {
            //lock (this)
                return Attributes.SelectMany(x => x).GetEnumerator();
        }

        public bool HasAttribute(AttributeType type)
        {
            //lock (this)
                return Attributes[(int)type].Count > 0;
        }

        public void RemoveAttribute(IAttribute attrib)
        {
            //lock (this)
            {
                Attributes[(int)attrib.AttributeType].Remove(attrib);
            }
        }

        public void RemoveAttributesByType(AttributeType type)
        {
            //lock (this)
            {
                Attributes[(int)type].Clear();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
