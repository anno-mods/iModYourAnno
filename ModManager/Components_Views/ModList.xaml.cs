using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
using ModManager_Classes;
using ModManager_Classes.src.Handlers;
using ModManager_Classes.src.Models;

namespace ModManager_Views
{
    /// <summary>
    /// Interaktionslogik für ModList.xaml
    /// </summary>
    public partial class ModList : UserControl, INotifyPropertyChanged
    {
        public LocalizedText InactiveText { get; } = TextManager.Instance.GetText("MODLIST_INACTIVE");
        public LocalizedText ActiveText { get; } = TextManager.Instance.GetText("MODLIST_ACTIVE");
        public LocalizedText ActivateText { get; } = TextManager.Instance.GetText("MODLIST_ACTIVATE");
        public LocalizedText DeactivateText { get; } = TextManager.Instance.GetText("MODLIST_DEACTIVATE");

        public Mod? CurrentlyDisplayedMod { get; private set; } = null;

        public ModDirectoryManager ModManager { get; private set; } = ModDirectoryManager.Instance;

        public bool ShowActivateButton {
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

        public ModList()
        {
            InitializeComponent();
            DataContext = this;
            OnSelectionChanged();
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnSelectionChanged();
        }

        private void OnSelectionChanged()
        {
            var selectedItems = ListBox_ModList.SelectedItems.Cast<Mod>().ToList();
            ShowActivateButton = selectedItems.Any(x => !x.Active);
            ShowDeactivateButton = selectedItems.Any(x => x.Active);

            CurrentlyDisplayedMod = ListBox_ModList.SelectedItems.Count > 0 ? ListBox_ModList.SelectedItems[ListBox_ModList.SelectedItems.Count - 1] as Mod : ListBox_ModList.SelectedItem as Mod;
        }

        private void ActivateButton_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (Mod m in ListBox_ModList.SelectedItems)
            {
                ModManager.Activate(m);
            }
            OnSelectionChanged(); 
        }

        private void DeactivateButton_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (Mod m in ListBox_ModList.SelectedItems)
            {
                ModManager.Deactivate(m);
            }
            OnSelectionChanged();
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


    [ValueConversion(typeof(bool), typeof(String))]
    internal class IconConverter : IValueConverter
    {
        public object Convert(object value, Type TargetType, object parameter, CultureInfo Culture)
        {
            bool b = (bool)value;
            return b ? "CheckBold" : "HighlightOff";
        }

        public object ConvertBack(object value, Type TargetType, object parameter, CultureInfo Culture)
        {
            string strValue = value as string;
            return strValue.Equals("CheckBold") ? true : false;
        }
    }

    [ValueConversion(typeof(bool), typeof(String))]
    internal class IconColorConverter : IValueConverter
    {
        public object Convert(object value, Type TargetType, object parameter, CultureInfo Culture)
        {
            bool b = (bool)value;
            return b ? "Green" : "Red";
        }

        public object ConvertBack(object value, Type TargetType, object parameter, CultureInfo Culture)
        {
            string strValue = value as string;
            return strValue.Equals("Green") ? true : false;
        }
    }
}
