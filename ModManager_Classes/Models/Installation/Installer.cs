using Imya.GithubIntegration;
using Imya.Models.NotifyPropertyChanged;
using Imya.Utils;
using System.Collections.ObjectModel;

namespace Imya.Models.Installation
{
    public class Installer
    {
        public ObservableCollection<IInstallation> RunningInstallations { get; } = new();

        public Installer() { }

        public void RemoveInstallation(IInstallation x)
        {
            RunningInstallations.Remove(x);
        }

        public Task<IInstallation> CreateModInstallationTask(GithubRepoInfo githubRepoInfo, ModInstallationOptions Options)
        {
            var installation = new ModGithubInstallation(githubRepoInfo, Options);
            RunningInstallations.Add(installation);
            return installation.Setup();
        }

        public Task<IInstallation> CreateModloaderInstallationTask()
        {
            var installation = new ModloaderInstallation();
            RunningInstallations.Add(installation);
            return installation.Setup();
        }

        public List<Task<IInstallation>> CreateInstallationTasks(IEnumerable<String> Filenames, ModInstallationOptions Options)
        {
            List<Task<IInstallation>> InstallationTasks = new();

            foreach (var Filename in Filenames)
            {
                if (!IsRunningInstallation(Filename))
                {
                    var InstallationTask = new ZipInstallation(Filename, Options);
                    RunningInstallations.Add(InstallationTask);
                    InstallationTasks.Add(InstallationTask.Setup());
                }
                else
                {
                    Console.WriteLine($"Installation Already Running: {Filename}");
                }
            }
            return InstallationTasks;
        }

        public async Task ProcessParallelAsync(List<Task<IInstallation>> installations)
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

        private bool IsRunningInstallation(String SourceFilepath) => RunningInstallations.Any(x => x is ZipInstallation && ((ZipInstallation)x).SourceFilepath.Equals(SourceFilepath));

    }
}
