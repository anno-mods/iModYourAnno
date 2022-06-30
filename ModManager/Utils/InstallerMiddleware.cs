using Imya.GithubIntegration;
using Imya.Models.Installation;
using Imya.Models.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.UI.Utils
{
    public enum InstallationResultType { SuccessfulInstallation, InstallationAlreadyRunning, Exception}

    public class InstallationResult
    {
        public InstallationResultType ResultType { get; init; }
        public InstallationException? Exception { get; init; }

        public InstallationResult(InstallationResultType _type) : this(_type, null) { }

        public InstallationResult(InstallationResultType _type, InstallationException? e)
        {
            ResultType=_type;
            Exception = e;
        }
    }
    
    internal class InstallerMiddleware
    {
        public InstallationStarter Installer { get; init; }

        public InstallerMiddleware(InstallationStarter installer)
        {
            Installer = installer;
        }

        public InstallerMiddleware()
        {
            Installer = new InstallationStarter();
        }

        public async Task<InstallationResult> RunZipInstallAsync(IEnumerable<String> Filenames, ModInstallationOptions Options)
        {
            var InstallationTasks = Installer.SetupZipInstallationTasks(Filenames, Options);
            //return is only okay here because the cleanup does not need to happen in this specific case.
            if (InstallationTasks.Count() == 0) return new InstallationResult(InstallationResultType.InstallationAlreadyRunning);
            await Installer.ProcessParallelAsync(InstallationTasks);
            foreach (var i in InstallationTasks)
            {
                Installer.CleanInstallation(i);
            }
            return new InstallationResult(InstallationResultType.SuccessfulInstallation);
        }

        public async Task<InstallationResult> RunGithubInstallAsync(GithubRepoInfo repo, ModInstallationOptions Options)
        {
            Task<IInstallation>? installation = null;
            try
            {
                installation = Installer.SetupModInstallationTask(repo, Options);
                if (installation is Task<IInstallation> valid_install)
                    await Installer.ProcessAsync(valid_install);
                else
                    return new InstallationResult(InstallationResultType.InstallationAlreadyRunning);
                return new InstallationResult(InstallationResultType.SuccessfulInstallation);
            }
            catch (InstallationException ex)
            {
                return new InstallationResult(InstallationResultType.Exception, ex);
            }
            finally
            {
                if (installation is Task<IInstallation>)
                    Installer.CleanInstallation(installation);
            }
        }

        public async Task<InstallationResult> RunModloaderInstallAsync()
        {
            Task<IInstallation>? installation = null;
            try
            {
                installation = Installer.SetupModloaderInstallationTask();
                if (installation is Task<IInstallation> valid_install)
                    await Installer.ProcessAsync(valid_install);
                else
                    return new InstallationResult(InstallationResultType.InstallationAlreadyRunning);
                return new InstallationResult(InstallationResultType.SuccessfulInstallation);
            }
            catch (InstallationException ex)
            {
                return new InstallationResult(InstallationResultType.Exception, ex);
            }
            finally
            {
                if (installation is Task<IInstallation>)
                    Installer.CleanInstallation(installation);
            }
        }
    }
}
