using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Imya.UI
{
    public class EmbeddedConsole : TextWriter
    {
        private TextBox TargetTextBox;
        private MainWindow Parent;

        public override Encoding Encoding => Encoding.Unicode;

        public EmbeddedConsole(TextBox t, MainWindow m)
        {
            TargetTextBox = t;
            Parent = m; 
        }

        private void WriteTimestamp()
        {
            TargetTextBox.Dispatcher.BeginInvoke(() =>
                TargetTextBox.Text += $"[LOG | { DateTime.Now.ToString("dd.MM.yy | hh:mm:ss")}]: "
            );
        }

        public override void Write(char c)
        {
            WriteTimestamp();
            TargetTextBox.Dispatcher.BeginInvoke(() =>
                TargetTextBox.Text += c
            );
        }

        public override void Write(String? s)
        {
            WriteTimestamp();
            TargetTextBox.Dispatcher.BeginInvoke(() =>
                TargetTextBox.Text += s
            );
        }

        public override void WriteLine(String? s)
        {
            WriteTimestamp();
            TargetTextBox.Dispatcher.BeginInvoke(() =>
                {
                    TargetTextBox.Text += s;
                    TargetTextBox.Text += Environment.NewLine;
                }
            );
        }
    }
}
