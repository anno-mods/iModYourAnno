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
        private ConcurrentDictionary<Guid, IAttribute> Attributes = new();

        public void AddAttribute(IAttribute attrib)
        {
            Attributes.TryAdd(Guid.NewGuid(), attrib);
        }

        public IAttribute? GetByType(AttributeType type)
        {
            return Attributes.Values.FirstOrDefault(x => x.AttributeType == type);
        }

        public IEnumerator<IAttribute> GetEnumerator()
        {
            return Attributes.Values.GetEnumerator();
        }

        public bool HasAttribute(AttributeType type)
        {
            return (Attributes.Values.Any(x => x.AttributeType == type));
        }

        public void RemoveAttribute(IAttribute attrib)
        {
            var items = Attributes.Where(x => x.Value == attrib).ToList();
            foreach (var item in items)
                Attributes.Remove(item.Key, out var _);
        }

        public void RemoveAttributesByType(AttributeType type)
        {
            var items = Attributes.Where(x => x.Value.AttributeType == type).ToList();
            foreach (var item in items)
                Attributes.Remove(item.Key, out var _);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Attributes.Values.GetEnumerator();
        }
    }
}
