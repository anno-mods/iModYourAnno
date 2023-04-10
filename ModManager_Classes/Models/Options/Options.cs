using Imya.Models.NotifyPropertyChanged;
using Imya.Services;
using Imya.Services.Interfaces;
using System.ComponentModel;

namespace Imya.Models.Options
{
    public interface IModloaderInstallationOptions 
    { 
        String UnpackDirectory { get; set; }
    }

    public class ModloaderInstallationOptions : IModloaderInstallationOptions
    {
        public String UnpackDirectory { get; set; }
        public ModloaderInstallationOptions(IImyaSetupService imyaSetupService) {
            UnpackDirectory = imyaSetupService.UnpackDirectoryPath;
        }
    }

    public interface IGithubDownloaderOptions
    {
        String DownloadDirectory { get; set; }
    }

    public class GithubDownloaderOptions : IGithubDownloaderOptions
    {
        public String DownloadDirectory { get; set; }
        public GithubDownloaderOptions(IImyaSetupService imyaSetupService)
        {
            DownloadDirectory = imyaSetupService.DownloadDirectoryPath;
        }
    }

    public interface IModInstallationOptions
    {
        String UnpackDirectory { get; set; }
    }

    public class ModInstallationOptions : PropertyChangedNotifier, IModInstallationOptions
    {
        public bool AllowOldToOverwrite  {
            get => _allowOldToOverwrite;
            set 
            {
                _allowOldToOverwrite = value;
                OnPropertyChanged(nameof(AllowOldToOverwrite));
            }
        }
        private bool _allowOldToOverwrite = false;
        public String UnpackDirectory { get; set; }

        public ModInstallationOptions(IImyaSetupService imyaSetupService)
        {
            UnpackDirectory = imyaSetupService.UnpackDirectoryPath;
        }
    }

    public class ModCollectionOptions
    {
        public bool Normalize { get; init; }
        public bool LoadImages { get; init; }
    }
}
