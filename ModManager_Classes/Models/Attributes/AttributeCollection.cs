using System.Collections;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;

namespace Imya.Models.Attributes
{
#if false
    public class AttributeCollection : ObservableCollection<IAttribute>
    {
        private object _lock = new Object();
        
        public void AddAttribute(IAttribute attrib)
        {
            if (attrib is null)
                return;
            lock (_lock)
            {
                if (!attrib.MultipleAllowed && this.Any(x => x.AttributeType == attrib.AttributeType))
                    return;
                Add(attrib);
            }
        }

        public IAttribute? GetByType(AttributeTypes type)
        {
            return this.FirstOrDefault(x => x.AttributeType == type);
        }

        public bool HasAttribute(AttributeTypes type)
        {
            return this.Any(x => x?.AttributeType == type);
        }

        public void RemoveAttribute(IAttribute attrib)
        {
            Remove(attrib);
        }

        public void RemoveAttributesByType(AttributeTypes type)
        {
            var items = this.Where(x => x.AttributeType == type).ToArray();
            foreach (var item in items)
                Remove(item);
        }
    }
#endif
}
