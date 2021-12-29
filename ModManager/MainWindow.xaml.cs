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
        public MainWindow()
        {
            InitializeComponent();
            SetUpEmbeddedConsole();
            Dashboard.MainViewChanged += ChangeMainWindow;
            Console.WriteLine("Modders Are gonna take over the World!!!");

            MinHeight = Settings.Default.MIN_WINDOW_HEIGHT;
            MinWidth = Settings.Default.MIN_WINDOW_WIDTH;
        }

        public void SetUpEmbeddedConsole()
        {
            Console.SetOut(new EmbeddedConsole(ConsoleLogTextBox.ConsoleOut, this));
            
        }

        private void ChangeMainWindow(int index)
        {
            if (index >= 0 && index <= 1)
            {
                WindowViewControl.SelectedIndex = index;
            }
        }
    }
}
