using Imya.GithubIntegration;
using Imya.GithubIntegration.StaticData;
using Imya.Models;
using System;
using System.Collections.Generic;
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

        public GithubRepoInfo SelectedRepo { get; init; }

        IRepositoryInfoProvider RepoInfoProvider = new StaticRepositoryInfoProvider();

        public GithubInstallPopup()
        {
            InitializeComponent();
            DataContext = this;

            OK_TEXT = new SimpleText("OK");
            CANCEL_TEXT = new SimpleText("Cancel");

            SelectedRepo = RepoInfoProvider.GetSingle();
            MESSAGE = new SimpleText(SelectedRepo.ToString());
        }

        private void OkayButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
