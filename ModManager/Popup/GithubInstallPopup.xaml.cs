using Imya.GithubIntegration;
using Imya.GithubIntegration.JsonData;
using Imya.GithubIntegration.StaticData;
using Imya.Models;
using Imya.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Imya.UI.Popup
{
    public partial class GithubInstallPopup : Window, INotifyPropertyChanged
    {
        public IText MESSAGE { get; set; }
        public IText OK_TEXT { get; set; }
        public IText CANCEL_TEXT { get; set; }


        public bool HasRepoSelection => SelectedRepo is not null;

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
        #endregion

        public ObservableCollection<GithubRepoInfo> AllRepositories;

        StaticReadmeProvider ReadmeProvider = new StaticReadmeProvider();

        public TextManager TextManager { get;} = TextManager.Instance;

        public GithubInstallPopup()
        {
            InitializeComponent();
            DataContext = this;

            OK_TEXT = new SimpleText("OK");
            CANCEL_TEXT = new SimpleText("Cancel");

            var repoInfoProvider = new AutoRepoInfoSource(GameSetupManager.Instance.ModindexLocation);
            AllRepositories = new ObservableCollection<GithubRepoInfo>(repoInfoProvider.GetAll());

            DisplayedRepositories = AllRepositories;

            RepoSelection.SelectionChanged += UpdateReadmeForSelection;
        }

        private void OkayButtonClick(object sender, RoutedEventArgs e)
        {
            SelectedRepo = RepoSelection.SelectedItem as GithubRepoInfo;
            DialogResult = true;
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private async void UpdateReadmeForSelection(object sender, SelectionChangedEventArgs e)
        { 
            var repoInfo = RepoSelection.SelectedItem as GithubRepoInfo;
            if (repoInfo is null) return;
            ReadmeText = await ReadmeProvider.GetReadmeAsync(repoInfo);

            SelectedRepo = repoInfo;
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
    }
}
