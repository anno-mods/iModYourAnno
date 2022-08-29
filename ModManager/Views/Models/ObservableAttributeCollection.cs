using Imya.Models.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Imya.UI.Models
{
    public class ObservableAttributeCollection : ObservableCollection<IAttribute>, IAttributeCollection
    {
        public void AddAttribute(IAttribute attrib)
        {
            if (!attrib.MultipleAllowed && this.Any(x => x.AttributeType == attrib.AttributeType))
                return;

            App.Current.Dispatcher.Invoke(() => this.Add(attrib));
        }

        public IAttribute? GetByType(AttributeType type)
        {
            return this.FirstOrDefault(x => x.AttributeType == type);
        }

        public bool HasAttribute(AttributeType type)
        {
            return this.Any(x => x.AttributeType == type);
        }

        public void RemoveAttribute(IAttribute attrib)
        {
            App.Current.Dispatcher.Invoke(() => this.Remove(attrib));
        }

        public void RemoveAttributesByType(AttributeType type)
        {
            var items = this.Where(x => x.AttributeType == type).ToArray();
            foreach(var item in items)
            {
                App.Current.Dispatcher.Invoke(() => this.Remove(item));
            }
                
        }
    }
}
