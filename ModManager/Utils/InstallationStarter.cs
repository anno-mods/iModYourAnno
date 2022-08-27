using Imya.GithubIntegration;
using Imya.GithubIntegration.Download;
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
    
    internal class InstallationStarter
    {
        public InstallationSetup Installer { get; init; }

        public InstallationStarter(InstallationSetup installer)
        {
            Installer = installer;
        }

        public InstallationStarter()
        {
            Installer = new InstallationSetup();
        }

        public async Task<IEnumerable<InstallationResult>> RunZipInstallAsync(IEnumerable<String> Filenames, ModInstallationOptions Options)
        {
            List<Task<IInstallation>> installations = new();
            List<InstallationResult> results = new();

            foreach (var Filename in Filenames)
            {
                var installation = Installer.SetupZipInstallationTask(Filename, Options);
                if (installation is Task<IInstallation> valid_install)
                {
                    installations.Add(valid_install);
                }
                else
                {
                    results.Add(new InstallationResult(InstallationResultType.InstallationAlreadyRunning));
                }
            }

            while (installations.Count > 0)
            {
                var installation = await Task.WhenAny(installations);
                try
                {
                    await Installer.ProcessAsync(installation);
                    results.Add(new InstallationResult(InstallationResultType.SuccessfulInstallation));
                }
                catch (InstallationException ex)
                {
                    results.Add(new InstallationResult(InstallationResultType.Exception, ex));
                }
                finally
                {
                    if (installation is Task<IInstallation>)
                        Installer.CleanInstallation(installation);
                    installations.Remove(installation);
                }
            }

            return results;
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
            catch (Exception e)
            {
                return new InstallationResult(InstallationResultType.Exception, new InstallationException("An unknown exception occured."));
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
