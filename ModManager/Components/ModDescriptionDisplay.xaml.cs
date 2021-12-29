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
using Imya.Models.ModMetadata;
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
        private bool _showDlcDeps;
        private bool _showImage;
        private double _descriptionTextWidth;
        private double _knownIssueTextWidth;

        private DlcId[] _DlcIds;
        #endregion

        #region Fields

        public DlcId[] DlcIds
        {
            get => _DlcIds;
            set
            {
                _DlcIds = value;
                OnPropertyChanged(nameof(DlcIds));
            }
        }
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

        public bool ShowDlcDeps
        {
            get => _showDlcDeps;
            set
            {
                _showDlcDeps = value;
                OnPropertyChanged(nameof(ShowDlcDeps));
            }
        }

        public bool ShowImage { 
            get=> _showImage;
            set
            { 
                _showImage = value;
                OnPropertyChanged(nameof(ShowImage));
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

        public TextManager TextManager { get; } = TextManager.Instance;

        public ModDescriptionDisplay()
        {
            InitializeComponent();
            DataContext = this;
            //TextManager.Instance.LanguageChanged += UpdateTextBoxes;
        }

        private DlcId[] GetDlcDependencies(Dlc[]? dependencies)
        {
            if (dependencies is not null)
            {
                return dependencies.Select(x => x.DLC).Where(x => x is DlcId).Select(x => (DlcId)x).OrderBy(x => x).ToArray();
            }
            else return new DlcId[0];
        }


        public void SetDisplayedMod(Mod m)
        {
            Mod = m;

            bool Exists = m is not null; 

            ShowKnownIssues = Exists ? m.HasKnownIssues : false ;
            ShowDescription = Exists ? m.HasDescription : false ;
            ShowCreatorName = Exists ? m.HasCreator : false ;
            ShowVersion = Exists ? m.HasVersion : false ;
            ShowDlcDeps = Exists ? m.HasDlcDependencies : false ;
            DlcIds = GetDlcDependencies(m?.DlcDependencies);

            //the default behavior for images is different: If the mod does not have an image, it will show a placeholder. 
            //Only hide the image in case there is no displayed mod.
            ShowImage = Exists;
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

        private void UpdateTextBoxes(ApplicationLanguage lang)
        {
            foreach (var item in DLC_Dependencies.Items)
            {
                ContentPresenter uiElement = (ContentPresenter)DLC_Dependencies.ItemContainerGenerator.ContainerFromItem(item);
                uiElement.ApplyTemplate();
                var Textblock = (TextBlock)uiElement.ContentTemplate.FindName("DLC_Dependency_TextboxTemplate", uiElement);
                BindingExpression bindingExpression = Textblock.GetBindingExpression(TextBlock.TextProperty);
                bindingExpression.UpdateSource();
            }
        }
        #endregion
    }
}
