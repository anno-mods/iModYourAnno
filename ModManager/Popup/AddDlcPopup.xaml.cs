using Imya.Enums;
using Imya.Models;
using Imya.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;

namespace Imya.UI.Popup
{
    /// <summary>
    /// Interaktionslogik für ProfilesPopup.xaml
    /// </summary>
    public partial class AddDlcPopup : Window, INotifyPropertyChanged 
    {
        public ObservableCollection<DlcId> Dlcs { get; private set; }

        private static String ProfilesDirectoryPath = GameSetupManager.Instance.GetProfilesDirectory();

        public DlcId[] SelectedIDs { get; private set; } = Array.Empty<DlcId>();

        public TextManager TextManager { get; private set; } = TextManager.Instance;

        public AddDlcPopup( IEnumerable<DlcId> dlcIds)
        {
            Dlcs = new ObservableCollection<DlcId>(dlcIds);

            InitializeComponent();
            DataContext = this;

            Title = TextManager.Instance["PROFILE_LOAD"].Text;

            DlcSelection.SelectionChanged += UpdateSelected;
        }

        private void UpdateSelected(object sender, SelectionChangedEventArgs e)
        {
            SelectedIDs = DlcSelection.SelectedItems.Cast<DlcId>().ToArray();
        }

        public void Accept()
        {

        }

        public void OkayButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        public void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler? PropertyChanged = delegate { };
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        protected void SetProperty<T>(ref T property, T value, [CallerMemberName] string propertyName = "")
        {
            property = value;
            OnPropertyChanged(propertyName);
        }
        #endregion
    }
}
