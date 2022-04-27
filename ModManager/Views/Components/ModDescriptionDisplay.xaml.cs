using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using Imya.Enums;
using Imya.Models;
using Imya.Models.ModMetadata;
using Imya.Utils;

using Markdown.Xaml;

namespace Imya.UI.Components
{
    /// <summary>
    /// Displays mod readme and some meta data.
    /// </summary>
    public partial class ModDescriptionDisplay : UserControl, INotifyPropertyChanged
    {
        #region Mod properties
        public Mod? Mod
        {
            get => _mod;
            private set => SetProperty(ref _mod, value);
        }
        private Mod? _mod;

        // Needs retrigger of OnPropertyChanged on language change
        public DlcId[] DlcIds
        {
            get => _dlcIds;
            private set => SetProperty(ref _dlcIds, value);
        }
        private DlcId[] _dlcIds = Array.Empty<DlcId>();
        #endregion

        #region Visibility
        public bool ShowKnownIssues {
            get => _showKnownIssues;
            set => SetProperty(ref _showKnownIssues, value);
        }
        private bool _showKnownIssues;

        public bool ShowDescription
        {
            get => _showDescription;
            set => SetProperty(ref _showDescription, value);
        }
        private bool _showDescription;

        public bool ShowCreatorName
        {
            get => _showCreatorName;
            private set => SetProperty(ref _showCreatorName, value);
        }
        private bool _showCreatorName;

        public bool ShowVersion
        {
            get => _showVersion;
            set => SetProperty(ref _showVersion, value);
        }
        private bool _showVersion;

        public bool ShowDlcDeps
        {
            get => _showDlcDeps;
            set => SetProperty(ref _showDlcDeps, value);
        }
        private bool _showDlcDeps;

        public bool ShowImage { 
            get => _showImage;
            set => SetProperty(ref _showImage, value);
        }
        private bool _showImage;

        public bool ShowModID
        {
            get => _showModID;
            set => SetProperty(ref _showModID, value);
        }
        private bool _showModID;
        #endregion

        public double DescriptionTextWidth {
            get => _descriptionTextWidth;
            set => SetProperty(ref _descriptionTextWidth, value);
        }
        private double _descriptionTextWidth;

        public double KnownIssueTextWidth {
            get => _knownIssueTextWidth;
            set => SetProperty(ref _knownIssueTextWidth, value);
        }
        private double _knownIssueTextWidth;

        public TextManager TextManager { get; } = TextManager.Instance;

        public ModDescriptionDisplay()
        {
            InitializeComponent();
            DataContext = this;
            TextManager.Instance.LanguageChanged += OnLanguageChanged;
        }

        public void SetDisplayedMod(Mod? mod)
        {
            Mod = mod;

            // TODO is it really necessary to trigger all invidiual fields?
            ShowKnownIssues = mod?.HasKnownIssues ?? false;
            ShowDescription = mod?.HasDescription ?? false;
            ShowCreatorName = mod?.HasCreator ?? false;
            ShowVersion = mod?.HasVersion ?? false;
            ShowDlcDeps = mod?.HasDlcDependencies ?? false;
            ShowModID = Properties.Settings.Default.ModCreatorMode && (mod?.HasModID ?? false);

            DlcIds = Mod?.Modinfo.DLCDependencies?.Where(x => x?.DLC != null).Select(x => (DlcId)x.DLC!).OrderBy(x => x).ToArray() ?? Array.Empty<DlcId>();

            // the default behavior for images is different:
            // If the mod does not have an image, it will show a placeholder. 
            // Only hide the image in case there is no displayed mod.
            ShowImage = mod != null;
        }

        public void OnCopyModIDClick(object sender, RoutedEventArgs e)
        {
            if (Mod == null) return;

            try
            {
                Clipboard.SetText(Mod.Modinfo.ModID);
            }
            catch (Exception)
            {
                Console.WriteLine("Could not access windows clipboard.");
            }
        }

        private void OnLanguageChanged(ApplicationLanguage language)
        {
            // force update of DLC ids
            DlcIds = DlcIds.ToArray();
        }


        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler? PropertyChanged = delegate { };
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        private void SetProperty<T>(ref T property, T value, [CallerMemberName] string propertyName = "")
        {
            property = value;
            OnPropertyChanged(propertyName);
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs s) 
        {
            DescriptionTextWidth = BaseGrid.ActualWidth > 20 ? BaseGrid.ActualWidth - 20 : 20;
            KnownIssueTextWidth = BaseGrid.ActualWidth > 50 ? BaseGrid.ActualWidth - 50 : 50;
        }
        #endregion
    }
}
