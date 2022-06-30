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

        public InstallationStarter() { }

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

        public IEnumerable<Task<IInstallation>> SetupZipInstallationTasks(IEnumerable<String> Filenames, ModInstallationOptions Options)
        {
            return SetupParallelInstallationTasks<String>(Filenames, Options,
                (Filename, Options) => SetupZipInstallationTask(Filename, Options));
        }

        public IEnumerable<Task<IInstallation>> SetupModInstallationTasks(IEnumerable<GithubRepoInfo> Repositories, ModInstallationOptions Options)
        {
            return SetupParallelInstallationTasks<GithubRepoInfo>(Repositories, Options,
                (Repo, Options) => SetupModInstallationTask(Repo, Options));
        }

        public async Task ProcessParallelAsync(IEnumerable<Task<IInstallation>> installations)
        {
            IEnumerable<IInstallation>? TaskResults = await Task.WhenAll(installations);

            foreach (var _task in TaskResults)
            {
                await _task.Finalize();
                RemoveInstallation(_task);
            }
        }

        public async Task ProcessAsync(Task<IInstallation> installation)
        {
            IInstallation TaskResult = await installation;
            await TaskResult.Finalize();
            RemoveInstallation(TaskResult);
        }

        private IEnumerable<Task<IInstallation>> SetupParallelInstallationTasks<T>(
            IEnumerable<T> Inputs,
            ModInstallationOptions Options,
            Func<T, ModInstallationOptions, Task<IInstallation>?> CreateInstallation)
        {
            foreach (T _t in Inputs)
            {
                var task = CreateInstallation.Invoke(_t, Options);
                if (task is Task<IInstallation> installation) yield return installation;
            }
        }

        private bool IsRunningInstallation(String SourceFilepath) => RunningInstallations.Any(x => x is ZipInstallation && ((ZipInstallation)x).SourceFilepath.Equals(SourceFilepath));
        private bool IsRunningInstallation(GithubRepoInfo g) => RunningInstallations.Any(x => x is GithubInstallation gitInstall && gitInstall.RepositoryToInstall.Equals(g));
    }
}
