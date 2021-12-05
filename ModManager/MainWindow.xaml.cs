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
using ModManager_Classes.src.Handlers;
using ModManager_Classes.src.Enums;

namespace ModManager_Views
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
            Console.WriteLine("Modders Are gonna take over the World!!!");

        }

        public void SetUpEmbeddedConsole()
        {
            Console.SetOut(new EmbeddedConsole(ConsoleLogTextBox.ConsoleOut, this));
        }
    }
}
