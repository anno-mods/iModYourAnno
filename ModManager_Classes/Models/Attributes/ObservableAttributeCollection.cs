using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.Attributes
{
    public class ObservableAttributeCollection : ObservableCollection<IAttribute>, IAttributeCollection
    {
        public void AddAttribute(IAttribute attrib)
        {
            this.Add(attrib);
        }

        public IAttribute? GetByType(AttributeType type)
        {
            return this.FirstOrDefault(x => x.AttributeType == type);
        }

        public void RemoveAttribute(IAttribute attrib)
        {
            this.Remove(attrib);
        }

        public void RemoveAttributesByType(AttributeType type)
        {
            var items = this.Where(x => x.AttributeType == type);
            foreach(var item in items)
                this.Remove(item);
        }
    }
}
