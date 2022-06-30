using Imya.Models.NotifyPropertyChanged;
using Imya.Utils;
using System.ComponentModel;

namespace Imya.Models.Options
{
    public class ModloaderInstallationOptions
    {
        public String UnpackDirectory { get; set; } = ImyaSetupManager.Instance.UnpackDirectoryPath;
    }

    public class GithubDownloaderOptions
    {
        public int DownloadBufferSize { get; set; } = 81920;
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);
        public String DownloadDirectory { get; set; } = ImyaSetupManager.Instance.DownloadDirectoryPath;
    }

    public class ModInstallationOptions : PropertyChangedNotifier
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
        public String UnpackDirectory { get; set; } = ImyaSetupManager.Instance.UnpackDirectoryPath;
    }

    public class ModCollectionOptions
    {
        public bool Normalize { get; init; }
        public bool LoadImages { get; init; }
    }
}
