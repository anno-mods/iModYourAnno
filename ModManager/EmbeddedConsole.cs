using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Imya.UI
{
    public class RedirectOutput : TextWriter
    {
        public override Encoding Encoding => Encoding.Unicode;
        private ILogger<RedirectOutput> _logger;
        public RedirectOutput(ILogger<RedirectOutput> logger ) 
        {
            _logger = logger;
        }

        public override void Write(char c) => _logger.LogInformation(""+c);
        public override void Write(String? s) => _logger.LogInformation(s);
        public override void WriteLine(String? s) => _logger.LogInformation(s);
    }
}
