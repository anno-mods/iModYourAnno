using Imya.GithubIntegration;
using Imya.Models.Installation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.UI.Utils
{
    public enum DownloadResultType { SuccessfulInstallation, InstallationAlreadyRunning, Exception}

    public class DownloadResult
    {
        public DownloadResultType ResultType { get; init; }
        public InstallationException? Exception { get; init; }

        public DownloadResult(DownloadResultType _type) : this(_type, null) { }

        public DownloadResult(DownloadResultType _type, InstallationException? e)
        {
            ResultType=_type;
            Exception = e;
        }
    }
    
    internal class InstallationMiddleware
    {
        public InstallationStarter Installer { get; init; }

        public InstallationMiddleware(InstallationStarter installer)
        {
            Installer = installer;
        }

        public InstallationMiddleware()
        {
            Installer = new InstallationStarter();
        }

        public async Task<DownloadResult> RunGithubInstallAsync(GithubRepoInfo repo, ModInstallationOptions Options)
        {
            Task<IInstallation>? installation = null;
            try
            {
                installation = Installer.SetupModInstallationTask(repo, Options);
                if (installation is Task<IInstallation> valid_install)
                    await Installer.ProcessAsync(valid_install);
                else
                    return new DownloadResult(DownloadResultType.InstallationAlreadyRunning);
                return new DownloadResult(DownloadResultType.SuccessfulInstallation);
            }
            catch (InstallationException ex)
            {
                return new DownloadResult(DownloadResultType.Exception, ex);
            }
            finally
            {
                if (installation is Task<IInstallation>)
                    Installer.CleanInstallation(installation);
            }
        }

        public async Task<DownloadResult> RunModloaderInstallAsync()
        {
            Task<IInstallation>? installation = null;
            try
            {
                installation = Installer.SetupModloaderInstallationTask();
                if (installation is Task<IInstallation> valid_install)
                    await Installer.ProcessAsync(valid_install);
                else
                    return new DownloadResult(DownloadResultType.InstallationAlreadyRunning);
                return new DownloadResult(DownloadResultType.SuccessfulInstallation);
            }
            catch (InstallationException ex)
            {
                return new DownloadResult(DownloadResultType.Exception, ex);
            }
            finally
            {
                if (installation is Task<IInstallation>)
                    Installer.CleanInstallation(installation);
            }
        }
    }
}
