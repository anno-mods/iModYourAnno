using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ModManager_Views
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

        public override void Write(char c)
        {
            TargetTextBox.Text += $"[LOG | { DateTime.Now.ToString("dd.MM.yy | hh:mm:ss")}] [LOG]: " + c;
        }

        public override void Write(String? s)
        {
            TargetTextBox.Text += $"[LOG | { DateTime.Now.ToString("dd.MM.yy | hh:mm:ss")}]: " + s;
        }

        public override void WriteLine(String? s)
        {
            Write(s);
            TargetTextBox.Text += Environment.NewLine;
        }
    }
}
