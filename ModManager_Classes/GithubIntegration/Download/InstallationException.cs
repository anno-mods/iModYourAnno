using Imya.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.GithubIntegration.Download
{
    [Obsolete]
    public class InstallationException : Exception
    {
        public String TextKey { get; }

        public InstallationException(String message, String textKey) : base(message) {
            TextKey = textKey;
        }

        public InstallationException(String message) : base(message) { }
        public InstallationException() : base() { }
    }
}
