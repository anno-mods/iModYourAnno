using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Imya.Models.ModMetadata;
using Imya.Utils;

namespace Imya.UI.Views
{
    /// <summary>
    /// Interaktionslogik für ModinfoCreatorView.xaml
    /// </summary>
    public partial class ModinfoCreatorView : UserControl, INotifyPropertyChanged
    {
        public TextManager TextManager { get; set; } = TextManager.Instance;

        public ModinfoCreationManager ModinfoCreationManager { get; set; } = ModinfoCreationManager.Instance;

        public ModinfoCreatorView()
        {
            DataContext = this;
            InitializeComponent();
        }


        public void OnNewClick(object sender, RoutedEventArgs e)
        {
            ModinfoCreationManager.Reset();
        }

        public void OnSaveClick(object sender, RoutedEventArgs e)
        {
            ModinfoCreationManager.Save("fuck.json");            
        }

        public void OnLoadClick(object sender, RoutedEventArgs e)
        {
            ModinfoCreationManager.Load("fuck.json");
        }

        #region INotifyPropertyChangedMembers

        public event PropertyChangedEventHandler? PropertyChanged = delegate { };

        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler is PropertyChangedEventHandler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
