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
using Imya.Texts;
using Imya.UI.Popup;
using Imya.UI.Utils;
using Imya.Utils;
using Anno.EasyMod.Metadata;

namespace Imya.UI.Views
{
    /// <summary>
    /// Interaktionslogik für ModinfoCreatorView.xaml
    /// </summary>
    public partial class ModinfoCreatorView : UserControl, INotifyPropertyChanged
    {
        public ITextManager TextManager { get; init; }

        public ModinfoFactory ModinfoFactory 
        {
            get => _factory;
            set
            {
                _factory = value;
                OnPropertyChanged(nameof(ModinfoFactory));
            }
        }

        private ModinfoFactory _factory;
        private PopupCreator _popupCreator;

        public ModinfoCreatorView(
            ITextManager textManager, 
            PopupCreator popupCreator)
        {
            TextManager = textManager;
            _popupCreator = popupCreator;
            DataContext = this;
            InitializeComponent();

            ModinfoFactory = new ModinfoFactory();
        }


        public void OnNewClick(object sender, RoutedEventArgs e)
        {
            ModinfoFactory.Reset();
        }

        public void OnSaveClick(object sender, RoutedEventArgs e)
        {
            Save("fuck.json");            
        }

        public void OnLoadClick(object sender, RoutedEventArgs e)
        {
            Load("fuck.json");
        }

        public void OnDlcDeleteClick(object sender, RoutedEventArgs e)
        {
            var but = sender as Button;
            var DataContext = but?.DataContext;

            var _id = DataContext as DlcId?;

            if (_id is not null) ModinfoFactory.RemoveDLC((DlcId)_id);
        }


        public void Load(String Filename)
        {
            throw new NotImplementedException(); 
        }

        public void Save(String Filename)
        {
            throw new NotImplementedException();
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

        private void OnDlcAddClick(object sender, RoutedEventArgs e)
        {
            var remaining = ModinfoFactory.GetRemainingDlcIds();
            AddDlcPopup popup = _popupCreator.CreateAddDlcPopup(remaining);
            popup.ShowDialog();

            if (popup.DialogResult is false) return;

            foreach (DlcId x in popup.SelectedIDs)
            {
                ModinfoFactory.AddDLC(x);
            }
        }
    }
}
