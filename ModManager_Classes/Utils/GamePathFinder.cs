using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Microsoft.Win32;

namespace Imya.Utils
{
    /// <summary>
    /// I'm not sure if WOW6432Node is true for everyone.
    /// Also, WOW6432Node\Ubisoft\Anno 1800 may be uplay specific.
    /// </summary>
    public class GamePathFinder
    {
        public static string? GetInstallDirFromRegistry() 
        {
            string installDirKey = @"SOFTWARE\WOW6432Node\Ubisoft\Anno 1800";
            using RegistryKey? key = Registry.LocalMachine.OpenSubKey(installDirKey);
            return key?.GetValue("InstallDir") as string;
        }
    }
}
