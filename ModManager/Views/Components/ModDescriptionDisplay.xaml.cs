using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using Imya.Enums;
using Imya.Models;
using Imya.Models.ModMetadata;
using Imya.Models.Mods;
using Imya.Texts;
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

        public bool ShowExtraInfo
        {
            get => _showExtraInfo;
            private set => SetProperty(ref _showExtraInfo, value);
        }
        private bool _showExtraInfo;

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

        public bool UseMarkdownDescription
        {
            get => _useMarkdownDescription;
            set => SetProperty(ref _useMarkdownDescription, value);
        }
        private bool _useMarkdownDescription = false;

        public String? MarkdownDescription
        {
            get => _markdownDescription;
            set => SetProperty(ref _markdownDescription, value);
        }
        private String? _markdownDescription;

        public ITextManager TextManager { get; init; }

        public double WindowWidth { get; private set; }

        public ModDescriptionDisplay(ITextManager textManager)
        {
            InitializeComponent();
            DataContext = this;
            TextManager = textManager;
            TextManager.LanguageChanged += OnLanguageChanged;
        }

        public void SetDisplayedMod(Mod? mod)
        {
            Mod = mod;

            // TODO is it really necessary to trigger all invidiual fields?
            ShowKnownIssues = mod?.HasKnownIssues ?? false;
            ShowDescription = mod?.HasDescription ?? false;
            ShowExtraInfo = mod is not null;
            ShowVersion = (mod?.HasVersion ?? false) || (mod?.HasCreator ?? false);
            ShowDlcDeps = mod?.HasDlcDependencies ?? false;
            ShowModID = Properties.Settings.Default.ModCreatorMode && (mod?.HasModID ?? false);

            DlcIds = Mod?.Modinfo.DLCDependencies?.Where(x => x?.DLC != null).Select(x => (DlcId)x.DLC!).OrderBy(x => x).ToArray() ?? Array.Empty<DlcId>();

            // the default behavior for images is different:
            // If the mod does not have an image, it will show a placeholder. 
            // Only hide the image in case there is no displayed mod.
            ShowImage = mod is not null;

            AdjustDocumentWidth();
            UpdateDescription();
        }

        public void UpdateDescription()
        {
            UseMarkdownDescription = false;
            MarkdownDescription = null;

            if (Mod is null || !Mod.HasDescription || Mod.Modinfo.Description?.Text is not String description)
                return;

            if (description.StartsWith("file::"))
            {
                 string descAsPath = Path.Combine(Mod.FullModPath, description.Substring(6));
                 if(File.Exists(descAsPath))
                     MarkdownDescription = File.ReadAllText(descAsPath);
            }
            else if (description.StartsWith("# "))
                MarkdownDescription = description;

            UseMarkdownDescription = MarkdownDescription is not null;
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

        private void AdjustDocumentWidth()
        {
            var document = DescriptionFlowViewer.Document;
            if (document is not null) document.PageWidth = WindowWidth;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs s)
        {
            WindowWidth = BaseGrid.ActualWidth;

            AdjustDocumentWidth();

            DescriptionTextWidth = WindowWidth > 20 ? WindowWidth - 20 : 20;
            KnownIssueTextWidth = WindowWidth > 50 ? WindowWidth - 50 : 50;
            
        }

        private void OnLanguageChanged(ApplicationLanguage language)
        {
            // force update of DLC ids
            DlcIds = DlcIds.ToArray();
            UpdateDescription();
        }


        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler? PropertyChanged = delegate { };
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        private void SetProperty<T>(ref T property, T value, [CallerMemberName] string propertyName = "")
        {
            property = value;
            OnPropertyChanged(propertyName);
        }
        #endregion
    }
}
