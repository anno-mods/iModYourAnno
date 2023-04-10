using Imya.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Utils
{
    /// <summary>
    /// Validator for game setup that checks maindata, modloader, etc. 
    /// </summary>
    public class InstallationValidator
    {
        private static int MAX_RDA_INDEX = 22;

        String ExecutableDir;
        String GameRootPath;

        public InstallationValidator(String _game_root_path)
        {
            GameRootPath = _game_root_path;
            ExecutableDir = Path.Combine(GameRootPath, "Bin", "Win64");
        }

        public ModloaderInstallationState CheckModloaderInstallState()
        {
            if (ExecutableDir == null) return ModloaderInstallationState.Uninstalled;

            var ubiPython = Path.Combine(ExecutableDir, "python35_ubi.dll");
            var python = Path.Combine(ExecutableDir, "python35.dll");
            var backup = Path.Combine(ExecutableDir, "modloader.dll");

            var executable = Path.Combine(ExecutableDir, "Anno1800.exe");
            if (File.Exists(python) && File.Exists(executable))
            {
                // when the executable is a lot newer than ubiPython, then it either got repaired or updated
                // chances are you need an update, but there's no concrete action that you could do here
                // IsPotentiallyOutdated = File.GetLastWriteTimeUtc(executable) > File.GetLastWriteTimeUtc(ubiPython).AddHours(1);

                // mod loader has python and ubiPython at roughly the same time
                // in case python is newer chances are it got repaired.
                // consider it to be not installed in that case.
                // note: will give false results if you repair right before you download a super fresh mod loader release
                // TODO add hash-based check. remove the false result, but is only applicable when downloaded via imya
                return File.GetLastWriteTimeUtc(python) <= File.GetLastWriteTimeUtc(ubiPython).AddMinutes(20) && File.Exists(ubiPython) ?
                    ModloaderInstallationState.Installed :
                    File.Exists(backup) ? ModloaderInstallationState.Deactivated : ModloaderInstallationState.Uninstalled;
            }
            return ModloaderInstallationState.Uninstalled;
        }

        public bool ModLoaderIsActive()
        {
            return
                File.Exists(Path.Combine(ExecutableDir, "python35.dll")) &&
                File.Exists(Path.Combine(ExecutableDir, "python35_ubi.dll"));
        }


        public bool MaindataIsValid()
        {
            String MaindataPath = Path.Combine(GameRootPath, "maindata");
            return
                Directory.Exists(MaindataPath) &&
                CheckMaindata(MaindataPath);
        }

        private bool CheckMaindata(String MaindataPath)
        {
            List<String> BuildPaths = new List<String>();

            for (int i = 0; i <= MAX_RDA_INDEX; i++)
                BuildPaths.Add($"data{i}.rda");

            return !BuildPaths.Any(maindata_rda => !Directory.Exists(Path.Combine(GameRootPath, MaindataPath, maindata_rda)));
        }
    }
}
