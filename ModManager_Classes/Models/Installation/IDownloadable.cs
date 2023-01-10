using Downloader;

namespace Imya.Models.Installation
{
    public interface IDownloadable
    {
        String DownloadTargetFilename { get; }
        String DownloadUrl { get; }
        long? DownloadSize { get; }
    }
 
}
