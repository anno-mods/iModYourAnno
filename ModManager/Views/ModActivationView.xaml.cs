using Imya.Models;
using Imya.Utils;
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

namespace Imya.UI.Views
{
    /// <summary>
    /// Interaktionslogik für ModActivationView.xaml
    /// </summary>
    public partial class ModActivationView : UserControl, INotifyPropertyChanged
    {
        public TextManager TextManager { get; } = TextManager.Instance;

        public ModDirectoryManager ModManager { get; private set; } = ModDirectoryManager.Instance;

        public bool ShowActivateButton
        {
            get { return _showActivateButton; }
            private set
            {
                _showActivateButton = value;
                OnPropertyChanged("ShowActivateButton");
            }
        }
        private bool _showActivateButton;

        public bool ShowDeactivateButton
        {
            get { return _showDeactivateButton; }
            private set
            {
                _showDeactivateButton = value;
                OnPropertyChanged("ShowDeactivateButton");
            }
        }
        private bool _showDeactivateButton;

        public ModActivationView()
        {
            InitializeComponent();
            DataContext = this;
            ModList.ModList_SelectionChanged += ModDescription.SetDisplayedMod;
            ModList.ModList_SelectionChanged += UpdateButtons;
        }

        private void ActivateButton_OnClick(object sender, RoutedEventArgs e)
        {
            ModList.ActivateSelection();
        }

        private void DeactivateButton_OnClick(object sender, RoutedEventArgs e)
        {
            ModList.DeactivateSelection();
        }

        private void UpdateButtons(Mod m)
        {
            ShowActivateButton = ModList.AnyInactiveSelected();
            ShowDeactivateButton = ModList.AnyActiveSelected();
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
