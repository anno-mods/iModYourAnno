using System;
using System.Windows;
using Imya.UI.Properties;

namespace Imya.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Properties.Settings Settings { get; } = Properties.Settings.Default;

        public MainViewController MainViewController { get; set; } = new MainViewController();

        public MainWindow()
        {
            InitializeComponent();
            SetUpEmbeddedConsole();

            Console.WriteLine("Modders gonna take over the world!!!");
            DataContext = this;
            MinHeight = Settings.Default.MinWindowHeight;
            MinWidth = Settings.Default.MinWindowWidth;

            Title = "iModYourAnno - Anno 1800 Mod Manager";

            MainViewController.SetView(View.MOD_ACTIVATION);
        }

        public void SetUpEmbeddedConsole()
        {
            Console.SetOut(new EmbeddedConsole(ConsoleLogTextBox.ConsoleOut, this));
        }
    }
}
