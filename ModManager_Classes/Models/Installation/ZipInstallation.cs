using Imya.Models.Options;
using Imya.Utils;

namespace Imya.Models.Installation
{
    public class ZipInstallation : Installation, IUnpackableInstallation
    {
        public String SourceFilepath { get; init; }
        public String UnpackTargetPath { get; init; }

        public ZipInstallation() 
        {
            HeaderText = TextManager.Instance.GetText("INSTALLATION_HEADER_MOD");
            AdditionalText = new SimpleText(SourceFilepath);
        }
        public override string ToString() => $"InstallationTask of {SourceFilepath}";
    }


}
