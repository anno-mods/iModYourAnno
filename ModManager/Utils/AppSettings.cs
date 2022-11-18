using Imya.Models.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.UI.Utils
{
    public class AppSettings
    {
        public static AppSettings Instance { get; set; }

        public AppSettings()
        {
            Instance ??= this;
        }

        public ModInstallationOptions InstallationOptions { get; } = new();
    }
}
