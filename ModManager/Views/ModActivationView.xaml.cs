using Imya.Models;
using Imya.Utils;
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

namespace Imya.UI.Views
{
    /// <summary>
    /// Interaktionslogik für ModActivationView.xaml
    /// </summary>
    public partial class ModActivationView : UserControl
    {
        public LocalizedText InactiveText { get; } = TextManager.Instance.GetText("MODLIST_INACTIVE");
        public LocalizedText ActiveText { get; } = TextManager.Instance.GetText("MODLIST_ACTIVE");

        public ModDirectoryManager ModManager { get; private set; } = ModDirectoryManager.Instance;

        public ModActivationView()
        {
            InitializeComponent();
            DataContext = this;
            ModList.ModList_SelectionChanged += ModDescription.SetDisplayedMod;
        }
    }
}
