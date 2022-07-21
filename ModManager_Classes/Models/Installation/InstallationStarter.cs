using Imya.GithubIntegration;
using Imya.Models.Options;
using Imya.Utils;
using System.Collections.ObjectModel;

namespace Imya.Models.Installation
{
    public class InstallationStarter
    {
        public ObservableCollection<IInstallation> RunningInstallations { get; } = new();
        private Dictionary<int, IInstallation> InstallationsById = new();

        Mutex FinalizeMutex;

        public InstallationStarter() {
            FinalizeMutex = new Mutex();
        }

        public void CleanInstallation(Task<IInstallation> _task)
        { 
            var installation = InstallationsById.GetValueOrDefault(_task.Id);
            if (installation is IInstallation valid_install)
            { 
                RemoveInstallation(valid_install);
                valid_install.CleanUp();
            }
        }

        private void RemoveInstallation(IInstallation x)
        {
            RunningInstallations.Remove(x);
        }

        public Task<IInstallation>? SetupModInstallationTask(GithubRepoInfo githubRepoInfo, ModInstallationOptions Options)
        {
            if (IsRunningInstallation(githubRepoInfo)) return null;
            var installation = new ModGithubInstallation(githubRepoInfo, Options);
            RunningInstallations.Add(installation);
            var task = installation.Setup();
            InstallationsById.Add(task.Id, installation);
            return task;
        }

        public Task<IInstallation>? SetupModloaderInstallationTask()
        {
            var installation = new ModloaderInstallation();
            RunningInstallations.Add(installation);
            var task = installation.Setup();
            InstallationsById.Add(task.Id, installation);
            return task;
        }

        public Task<IInstallation>? SetupZipInstallationTask(String Filename, ModInstallationOptions Options)
        {
            if (IsRunningInstallation(Filename)) return null;
            var installation = new ZipInstallation(Filename, Options);
            RunningInstallations.Add(installation);
            var task = installation.Setup();
            InstallationsById.Add(task.Id, installation);
            return task;
        }

        public async Task ProcessAsync(Task<IInstallation> installation)
        {
            IInstallation TaskResult = await installation;

            FinalizeMutex.WaitOne();
            await TaskResult.Finalize();
            FinalizeMutex.ReleaseMutex();

            RemoveInstallation(TaskResult);
        }

        private bool IsRunningInstallation(String SourceFilepath) => RunningInstallations.Any(x => x is ZipInstallation && ((ZipInstallation)x).SourceFilepath.Equals(SourceFilepath));
        
        private bool IsRunningInstallation(GithubRepoInfo g) => RunningInstallations.Any(x => x is GithubInstallation gitInstall && gitInstall.RepositoryToInstall.Equals(g));
    }
}
