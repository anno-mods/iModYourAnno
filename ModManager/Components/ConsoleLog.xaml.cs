using System.Windows.Controls;

namespace Imya_UI.Components
{
    /// <summary>
    /// Interaktionslogik für Console.xaml
    /// </summary>
    public partial class ConsoleLog : UserControl
    {
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
