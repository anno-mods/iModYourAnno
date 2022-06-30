using Imya.GithubIntegration;
using Imya.GithubIntegration.StaticData;
using Imya.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Imya.UI.Popup
{
    /// <summary>
    /// Interaktionslogik für GenericOkayPopup.xaml
    /// </summary>
    public partial class GithubInstallPopup : Window
    {
        public IText MESSAGE { get; set; }
        public IText OK_TEXT { get; set; }
        public IText CANCEL_TEXT { get; set; }

        public GithubRepoInfo? SelectedRepo { get; private set; }

        public bool HasRepoSelection => SelectedRepo is not null;
        public ObservableCollection<GithubRepoInfo> AllRepositories { get; private set;}

        IRepositoryInfoProvider RepoInfoProvider = new StaticRepositoryInfoProvider();

        public GithubInstallPopup()
        {
            InitializeComponent();
            DataContext = this;

            OK_TEXT = new SimpleText("OK");
            CANCEL_TEXT = new SimpleText("Cancel");

            AllRepositories = new ObservableCollection<GithubRepoInfo>(RepoInfoProvider.GetAll());
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
    }
}
