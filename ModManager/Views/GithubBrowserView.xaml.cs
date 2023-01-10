using Imya.GithubIntegration;
using Imya.GithubIntegration.Download;
using Imya.GithubIntegration.JsonData;
using Imya.GithubIntegration.StaticData;
using Imya.Models;
using Imya.Models.Installation;
using Imya.UI.Popup;
using Imya.UI.Utils;
using Imya.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

namespace Imya.UI.Views
{
    public partial class GithubBrowserView : UserControl, INotifyPropertyChanged, IViewPage
    {
        public IText MESSAGE { get; set; }
        public IText OK_TEXT { get; set; }
        public IText CANCEL_TEXT { get; set; }

        #region Notifying
        public ObservableCollection<GithubRepoInfo> DisplayedRepositories 
        { 
            get => _displayedRepositories;
            private set => SetProperty(ref _displayedRepositories, value);
        }
        private ObservableCollection<GithubRepoInfo> _displayedRepositories;

        public String? ReadmeText
        {
            get => _readmeText;
            set => SetProperty(ref _readmeText, value);
        }
        private String? _readmeText;

        public GithubRepoInfo? SelectedRepo {
            get => _selectedRepo;
            private set => SetProperty(ref _selectedRepo, value);
        }
        private GithubRepoInfo? _selectedRepo;

        public bool CanAddToDownloads
        {
            get => _canAddToDownloads;
            set => SetProperty(ref _canAddToDownloads, value);
        }
        private bool _canAddToDownloads;
        #endregion

        public ObservableCollection<GithubRepoInfo> AllRepositories;
        private readonly StaticReadmeProvider ReadmeProvider = new();

        public TextManager TextManager { get;} = TextManager.Instance;
        public InstallationManager InstallationManager { get; } = InstallationManager.Instance;

        public GithubBrowserView()
        {
            DataContext = this;
            InitializeComponent();
            OK_TEXT = new SimpleText("Download");
            CANCEL_TEXT = new SimpleText("Cancel");

            InstallationManager.InstallationCompleted += ValidateCanAddToDownloads;
        }

        public void OnLoad()
        {
            var repoInfoProvider = new AutoRepoInfoSource(AppSettings.Instance.ModindexLocation);
            AllRepositories = new ObservableCollection<GithubRepoInfo>(repoInfoProvider.GetAll());
            DisplayedRepositories = AllRepositories;
        }

        private async void OkayButtonClick(object sender, RoutedEventArgs e)
        {
            SelectedRepo = RepoSelection.SelectedItem as GithubRepoInfo;
            if (SelectedRepo is null || InstallationView.Instance is null)
                return;

            try
            {
                var install = await GithubInstallationBuilder
                    .Create()
                    .WithRepoInfo(SelectedRepo)
                    .BuildAsync();

                InstallationManager.EnqueueGithubInstallation(install);
                ValidateCanAddToDownloads();
            }
            catch (InstallationException ex)
            {
                PopupCreator.CreateGithubExceptionPopup(ex).ShowDialog();
            }
        }

        private async void OnInstallFromZipAsync(object sender, RoutedEventArgs e)
        {
            if (ModCollection.Global is null) return;

            var dialog = CreateOpenFileDialog();
            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            foreach (String filename in dialog.FileNames)
            {
                var install = ZipInstallationBuilder
                    .Create()
                    .WithSource(filename)
                    .Build();

                InstallationManager.EnqueueZipInstallation(install);
            }
        }


        private System.Windows.Forms.OpenFileDialog CreateOpenFileDialog()
        {
            return new System.Windows.Forms.OpenFileDialog
            {
                Filter = "Zip Archives (*.zip)|*.zip",
                RestoreDirectory = true, // TODO keep location separate from game path dialog, it's annoying!
                Multiselect = true
            };
        }


        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            MainViewController.Instance.GoToLastView();
        }

        private async void OnRepoSelectionChanged(object sender, SelectionChangedEventArgs e)
        { 
            var repoInfo = RepoSelection.SelectedItem as GithubRepoInfo;
            if (repoInfo is null) return;
            SelectedRepo = repoInfo;

            ValidateCanAddToDownloads();
            ReadmeText = await ReadmeProvider.GetReadmeAsync(SelectedRepo);

        }

        private void ValidateCanAddToDownloads()
        {
            CanAddToDownloads = RepoSelection is not null && !InstallationManager.Instance.IsProcessingInstallWithID(SelectedRepo.GetID());
        }

        public void Filter(IEnumerable<string> keywords)
        { 
            var selection = AllRepositories.Where(repoInfo =>
               keywords.Any(x => repoInfo.Name.Contains(x, StringComparison.InvariantCultureIgnoreCase))
               || keywords.Any(x => repoInfo.Owner.Contains(x, StringComparison.InvariantCultureIgnoreCase))
            );

            DisplayedRepositories = selection.Count() > 0 ? new(selection) : AllRepositories;
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


        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textbox = sender as TextBox;
            if (textbox is not TextBox valid_textbox) return;
            var keywords = valid_textbox.Text.Split(' ').Where(x => x != String.Empty);

            Filter(keywords);
        }

        private void OnOpenGithubClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(@$"https://github.com/{SelectedRepo!.Owner}/{SelectedRepo!.Name}") { UseShellExecute = true});
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not open Repository on Github: {SelectedRepo!.Owner}/{SelectedRepo!.Name}");
            }

        }

        #region hacky_image_size_correction
        private async void DescriptionFlowViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //rerender the flow document cuz images 
            await Application.Current.Dispatcher.BeginInvoke(
                () =>
                {
                    if (DescriptionFlowViewer.Document is null)
                        return;
                    DescriptionFlowViewer.Document.PageWidth = DescriptionFlowViewer.Document.PageWidth;
                });
        }
        #endregion

    }
}
