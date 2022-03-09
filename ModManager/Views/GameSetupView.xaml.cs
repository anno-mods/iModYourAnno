﻿using Imya.UI.Popup;
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
    /// Interaktionslogik für GameSetupView.xaml
    /// </summary>
    public partial class GameSetupView : UserControl
    {
        InstallationManager installationManager = InstallationManager.Instance; 

        public GameSetupView()
        {
            InitializeComponent();
        }

        public async void ModloaderInstalled_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Started installing Modloader");
            ModloaderDownloadButton.IsEnabled = false;
            await installationManager.InstallModLoaderAsync();
            ModloaderDownloadButton.IsEnabled = true;

            GenericOkayPopup inputDialog = new GenericOkayPopup();
            inputDialog.ShowDialog();
        }
    }
}