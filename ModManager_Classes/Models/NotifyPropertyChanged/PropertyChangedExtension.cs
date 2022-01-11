using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.PropertyChanged
{
    public static class PropertyChangedExtension
    {
        //Please microsoft, this is standard stuff for WPF. why don't UserControls come with a callable method by default?
        public static void NotifyPropertyChanged(this INotifyPropertyChanged notifier, PropertyChangedEventHandler PropertyChanged, string propertyName)
        {
            if (PropertyChanged is PropertyChangedEventHandler)
            {
                PropertyChanged(notifier, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
