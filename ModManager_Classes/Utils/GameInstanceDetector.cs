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
    /// An insufficient attempt to autodetect game instances on windows without file crawling
    /// 
    /// Don't use this yet.
    /// 
    /// Thanks.
    /// </summary>
    public class GameInstanceManager
    {
        public ObservableCollection<String> GameInstances { get; set; }

        public int GameInstanceCount { get => GameInstances.Count; }

        public GameInstanceManager()
        {
            GameInstances = new ObservableCollection<String>();
        }

        private IEnumerable<RegistryKey> FetchUninstallableProgramsFromWin()
        {
            String RegistryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(RegistryKey))
            {
                foreach (String subkey_name in key.GetSubKeyNames())
                {
                    using (RegistryKey subKey = key.OpenSubKey(subkey_name))
                    {
                        yield return subKey;
                    }
                }
            }
        }

        private IEnumerable<String> GetNames(IEnumerable<RegistryKey> seq)
        {
            return seq.Select(x => (String)x.GetValue("DisplayName"));
        }


        public IEnumerable<String> SearchInstances() 
        {
            var all = FetchUninstallableProgramsFromWin();

            var names = all.Select(x => (String)x.GetValue("DisplayName"));

            var fuck = names.Where( y => y is not null && y.StartsWith("Anno") );

            return fuck;
        }
    }
}
