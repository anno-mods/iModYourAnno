using System.Windows.Controls;

namespace Imya.UI.Components
{
    /// <summary>
    /// Interaktionslogik für Console.xaml
    /// </summary>
    public partial class ConsoleLog : UserControl
    {
        public TextBox Console { get => ConsoleOut; }
        public ConsoleLog()
        {
            InitializeComponent();
        }

        private void ConsoleLog_TextChanged(object sender, TextChangedEventArgs e)
        {
            ScrollPane.ScrollToBottom();
        }
    }
}
