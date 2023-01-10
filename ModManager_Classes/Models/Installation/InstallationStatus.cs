using Imya.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.Installation
{
    internal class InstallationStatus : IInstallationStatus
    {
        public static readonly InstallationStatus NotStarted = new("ZIP_NOTSTARTED");
        public static readonly InstallationStatus Unpacking = new("ZIP_UNPACKING");
        public static readonly InstallationStatus MovingFiles = new("ZIP_MOVING");
        public static readonly InstallationStatus Downloading = new("INSTALL_DOWNLOAD");

        private readonly string _value;
        private InstallationStatus(string value)
        {
            _value = value;
        }

        public IText Localized => TextManager.Instance[_value];
    }
}
