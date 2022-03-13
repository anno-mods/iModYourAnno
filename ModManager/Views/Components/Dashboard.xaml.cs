using Imya.Models;
using Imya.Utils;
using System.Windows;
using System.Windows.Controls;
using Imya.Enums;
using System.ComponentModel;
using System.Windows.Data;
using System;
using System.Globalization;
using Imya.UI.Views;
using Imya.UI.Utils;

namespace Imya.UI.Components
{
    /// <summary>
    /// Interaktionslogik für Dashboard.xaml
    /// </summary>
    public partial class Dashboard : UserControl
    {
        public TextManager TextManager { get; } = TextManager.Instance;
        public Properties.Settings Settings { get; } = Properties.Settings.Default;
        public GameSetupManager GameSetupManager { get; } = GameSetupManager.Instance;
        public MainViewController MainViewController { get; } = MainViewController.Instance;


        public Dashboard()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void SettingsClick(object sender, RoutedEventArgs e)
        {
            MainViewController.SetView(View.SETTINGS);
        }

        public void ModManagementClick(object sender, RoutedEventArgs e)
        {
            MainViewController.SetView(View.MOD_ACTIVATION);
        }

        public void GameSetupClick(object sender, RoutedEventArgs e)
        {
            MainViewController.SetView(View.GAME_SETUP);
        }

        public void ModTweakerClick(object sender, RoutedEventArgs e)
        {
            MainViewController.SetView(View.TWEAKER);
        }

        public void MetadataClick(object sender, RoutedEventArgs e)
        {
            MainViewController.SetView(View.MODINFO_CREATOR);
        }

        public void ModInstallationClick(object sender, RoutedEventArgs e)
        {
            MainViewController.SetView(View.DUMMY);
        }

        public void StartGameClick(object sender, RoutedEventArgs e)
        {
            GameSetupManager.Instance.StartGame();
        }
    }
}
