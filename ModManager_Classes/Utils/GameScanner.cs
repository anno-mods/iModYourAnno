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
        public string? GetInstallDirFromRegistry() 
        {
            string installDirKey = @"SOFTWARE\WOW6432Node\Ubisoft\Anno 1800";
            using RegistryKey? key = Registry.LocalMachine.OpenSubKey(installDirKey);
            return key?.GetValue("InstallDir") as string;
        }

        public bool TryFetchRunningGame(out Process? _process)
        {
            _process = Process.GetProcessesByName("Anno1800").FirstOrDefault();
            return _process is not null;
        }


        /// <summary>
        /// Asynchronously searches for a running Anno 1800 process 
        /// </summary>
        /// <param name="TimeoutInSeconds"></param>
        /// <returns></returns>
        public async Task<Process?> ScanForRunningGameAsync(int TimeoutInSeconds)
        {
            Process? process = null;

            using var periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(1));
            for (int i = TimeoutInSeconds;
                    process is null &&
                    await periodicTimer.WaitForNextTickAsync()
                    && i > 0; i--)
            {
                TryFetchRunningGame(out process);
            }

            Console.WriteLine(process is not null ? "running process found!" : "failed to retrieve any running process");
            return process;
        }
    }
}
