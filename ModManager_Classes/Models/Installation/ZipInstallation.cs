using Imya.Models.Options;
using Imya.Utils;

namespace Imya.Models.Installation
{
    public class ZipInstallation : Installation, IUnpackable
    {
        public String SourceFilepath { get; init; }
        public String UnpackTargetPath { get; init; }

        public new ZipInstallationStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        private ZipInstallationStatus _status = ZipInstallationStatus.NotStarted;

        public ZipInstallation() 
        {
            HeaderText = TextManager.Instance.GetText("INSTALLATION_HEADER_MOD");
            AdditionalText = new SimpleText(SourceFilepath);
        }
        public override string ToString() => $"InstallationTask of {SourceFilepath}";
    }

    public class ZipInstallationStatus : IInstallationStatus
    {
        public static readonly ZipInstallationStatus NotStarted = new("ZIP_NOTSTARTED");
        public static readonly ZipInstallationStatus Unpacking = new("ZIP_UNPACKING");
        public static readonly ZipInstallationStatus MovingFiles = new("ZIP_MOVING");

        private readonly string _value;
        private ZipInstallationStatus(string value)
        {
            _value = value;
        }

        public IText Localized => TextManager.Instance[_value];
    }


}
