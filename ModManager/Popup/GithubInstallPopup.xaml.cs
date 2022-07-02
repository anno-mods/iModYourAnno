using Imya.GithubIntegration;
using Imya.GithubIntegration.StaticData;
using Imya.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace Imya.UI.Popup
{
    /// <summary>
    /// Interaktionslogik für GenericOkayPopup.xaml
    /// </summary>
    public partial class GithubInstallPopup : Window, INotifyPropertyChanged
    {
        public IText MESSAGE { get; set; }
        public IText OK_TEXT { get; set; }
        public IText CANCEL_TEXT { get; set; }

        public GithubRepoInfo? SelectedRepo { get; private set; }

        public bool HasRepoSelection => SelectedRepo is not null;
        public ObservableCollection<GithubRepoInfo> AllRepositories { get; private set; }

        StaticReadmeProvider ReadmeProvider = new StaticReadmeProvider();

        public String? ReadmeText { 
            get => _readmeText; 
            set => SetProperty(ref _readmeText, value); 
        }
        private String? _readmeText;

        IRepoInfoSource RepoInfoProvider = new StaticRepoInfoSource();

        public GithubInstallPopup()
        {
            InitializeComponent();
            DataContext = this;

            OK_TEXT = new SimpleText("OK");
            CANCEL_TEXT = new SimpleText("Cancel");

            AllRepositories = new ObservableCollection<GithubRepoInfo>(RepoInfoProvider.GetAll());

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
