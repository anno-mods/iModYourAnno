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
using Imya.Models;
using Imya.Utils;

namespace Imya.UI.Components
{
    /// <summary>
    /// Interaktionslogik für ModDescriptionDisplay.xaml
    /// </summary>
    public partial class ModDescriptionDisplay : UserControl, INotifyPropertyChanged
    {
        #region FieldBacking
        private Mod _mod;
        private bool _showKnownIssues;
        private double _descriptionTextWidth;
        private double _knownIssueTextWidth;
        #endregion

        #region Fields
        public Mod Mod
        {
            get => _mod;
            private set
            {
                _mod = value;
                OnPropertyChanged(nameof(Mod));
            }
        }
        

        public bool ShowKnownIssues {
            get => _showKnownIssues;
            set
            {
                _showKnownIssues = value;
                OnPropertyChanged(nameof(ShowKnownIssues));
            }
        }
        

        public double DescriptionTextWidth {
            get => _descriptionTextWidth;
            set
            { 
                _descriptionTextWidth = value;
                OnPropertyChanged(nameof(DescriptionTextWidth));
            }
        }
        

        public double KnownIssueTextWidth {
            get => _knownIssueTextWidth;
            set
            {
                _knownIssueTextWidth = value;
                OnPropertyChanged(nameof(KnownIssueTextWidth));
            }
        }

        #endregion

        //Texts 
        private LocalizedText NoVersion = TextManager.Instance.GetText("MODDISPLAY_NO_VERSION");
        private LocalizedText NoDescription = TextManager.Instance.GetText("MODDISPLAY_NO_DESCRIPTION");

        public ModDescriptionDisplay()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void SetDisplayedMod(Mod m)
        {
            Mod = m;
            ShowKnownIssues = m?.KnownIssues is LocalizedText[];
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

        private void OnSizeChanged(object sender, SizeChangedEventArgs s)
        {
            UpdateTextboxWidths();
        }

        private void UpdateTextboxWidths()
        {
            DescriptionTextWidth = BaseGrid.ActualWidth > 20 ? BaseGrid.ActualWidth - 20 : 20;
            KnownIssueTextWidth = BaseGrid.ActualWidth > 36 ? BaseGrid.ActualWidth - 36 : 36;
        }
        #endregion
    }
}
