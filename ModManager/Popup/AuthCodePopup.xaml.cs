using Imya.Models;
using Imya.Utils;
using System;
using System.Collections.Generic;
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

namespace Imya.UI.Popup
{
    /// <summary>
    /// Interaktionslogik für GenericOkayPopup.xaml
    /// </summary>
    public partial class AuthCodePopup : Window
    {
        public IText MESSAGE { get; set; }
        public IText CANCEL_TEXT { get; set; }

        public String AuthCode { get; private set; }

        public AuthCodePopup(String authcode)
        {
            InitializeComponent();
            DataContext = this;
            AuthCode = authcode;
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
