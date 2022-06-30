using Imya.Models.NotifyPropertyChanged;
using Imya.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.Options
{
    public struct ModloaderInstallationOptions
    {
        public String UnpackDirectory { get; set; } = ImyaSetupManager.Instance.UnpackDirectoryPath;
    }

    public struct GithubDownloaderOptions
    {
        public int DownloadBufferSize { get; set; } = 81920;
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);
        public String DownloadDirectory { get; set; } = ImyaSetupManager.Instance.DownloadDirectoryPath;
    }

    public struct ModInstallationOptions
    {
        public bool AllowOldToOverwrite  { get; set; } = false;
        public String UnpackDirectory { get; set; } = ImyaSetupManager.Instance.UnpackDirectoryPath;
    }

    public struct ModCollectionOptions
    {
        public bool Normalize { get; init; }
        public bool LoadImages { get; init; }
    }
}
