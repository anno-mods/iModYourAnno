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
using Imya.Enums;
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
        private bool _showDescription;
        private bool _showCreatorName;
        private bool _showVersion;
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

        public bool ShowDescription
        {
            get => _showDescription;
            set 
            {
                _showDescription = value;
                OnPropertyChanged(nameof(ShowDescription));
            }
        }

        public bool ShowCreatorName
        {
            get => _showCreatorName;
            set 
            {
                _showCreatorName = value;
                OnPropertyChanged(nameof(ShowCreatorName));
            }
        }

        public bool ShowVersion
        {
            get => _showVersion;
            set
            { 
                _showVersion = value;
                OnPropertyChanged(nameof(ShowVersion));
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

        private LocalizedText GetDlcText(DlcId dlc)
        {
            switch (dlc)
            {
                case DlcId.SunkenTreasures: return TextManager.Instance.GetText("DLC_SUNKENTREASURES");
                case DlcId.Botanica: return TextManager.Instance.GetText("DLC_BOTANICA");
                case DlcId.ThePassage : return TextManager.Instance.GetText("DLC_PASSAGE");
                case DlcId.SeatOfPower: return TextManager.Instance.GetText("DLC_SEATOFPOWER");
                case DlcId.BrightHarvest: return TextManager.Instance.GetText("DLC_BRIGHTHARVEST");
                case DlcId.LandOfLions: return TextManager.Instance.GetText("DLC_LANDOFLIONS");
                default: return new LocalizedText("ID unset");
            }
        }

        public void SetDisplayedMod(Mod m)
        {
            Mod = m;
            ShowKnownIssues = m?.KnownIssues is LocalizedText[];
            ShowDescription = m?.Description is LocalizedText;
            ShowCreatorName = m?.CreatorName is String;
            ShowVersion = m?.Version is String; 
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
            KnownIssueTextWidth = BaseGrid.ActualWidth > 50 ? BaseGrid.ActualWidth - 50 : 50;
        }
        #endregion
    }
}
