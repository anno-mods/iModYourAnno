using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.PropertyChanged
{
    public abstract class PropertyChangedNotifier : INotifyPropertyChanged
    {
        //Even less code duping if we can inherit. I know - am lazy.
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler? PropertyChanged = delegate { };

        public void OnPropertyChanged(string propertyName)
        {
            this.NotifyPropertyChanged(PropertyChanged, propertyName);
        }
        #endregion
    }
}
