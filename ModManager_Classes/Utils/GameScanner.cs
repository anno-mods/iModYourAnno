using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.Diagnostics;

namespace Imya.Utils
{
    /// <summary>
    /// I'm not sure if WOW6432Node is true for everyone.
    /// Also, WOW6432Node\Ubisoft\Anno 1800 may be uplay specific.
    /// </summary>
    public class GameScanner
    {
        public static string? GetInstallDirFromRegistry() 
        {
            string installDirKey = @"SOFTWARE\WOW6432Node\Ubisoft\Anno 1800";
            using RegistryKey? key = Registry.LocalMachine.OpenSubKey(installDirKey);
            return key?.GetValue("InstallDir") as string;
        }

        public static bool TryGetRunningGame(out Process? _process)
        {
            _process = Process.GetProcessesByName("Anno1800").FirstOrDefault();
            return _process is not null;
        }
    }
}
