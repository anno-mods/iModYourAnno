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
using ModManager_Classes;
using ModManager_Classes.src.Handlers;
using System.Linq;
using ModManager_Classes.src.Models;

namespace ModManager_Views
{
    /// <summary>
    /// Interaktionslogik für ModList.xaml
    /// </summary>
    public partial class ModList : UserControl
    {

        public Mod? CurrentlyDisplayedMod { get; private set; } = null;

        public ModList()
        {
            DataContext = ModDirectoryManager.Instance;
            InitializeComponent();
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListBox_ModList.Items.Count > 0 && ListBox_ModList.SelectedIndex >= 0)
            {

            }
            CurrentlyDisplayedMod = ListBox_ModList.SelectedItems.Count > 0 ? ListBox_ModList.SelectedItems[ListBox_ModList.SelectedItems.Count - 1] as Mod : ListBox_ModList.SelectedItem as Mod;
        }

    }
}
