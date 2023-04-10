using Imya.Models.Installation.Interfaces;
using Imya.Models.Options;
using Imya.Utils;

namespace Imya.Models.Installation
{
    public class ZipInstallation : Installation, IUnpackableInstallation
    {
        public String SourceFilepath { get; init; }
        public String UnpackTargetPath { get; init; }

        public ZipInstallation() { }

        public override string ToString() => $"InstallationTask of {SourceFilepath}";
    }


}
