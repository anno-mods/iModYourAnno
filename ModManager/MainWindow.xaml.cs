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
        public SettingsManager SettingsManager { get; } = SettingsManager.Instance;

        public MainWindow()
        {
            InitializeComponent();
            SetUpEmbeddedConsole();
            Console.WriteLine("Modders Are gonna take over the World!!!");
            DataContext = this;
            MinHeight = Settings.Default.MIN_WINDOW_HEIGHT;
            MinWidth = Settings.Default.MIN_WINDOW_WIDTH;

        }

        public void SetUpEmbeddedConsole()
        {
            Console.SetOut(new EmbeddedConsole(ConsoleLogTextBox.ConsoleOut, this));
        }
    }
}
