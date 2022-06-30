using Imya.Models.NotifyPropertyChanged;
using Imya.Models.Options;
using Imya.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.Installation
{
    public class ZipInstallation : Installation, IModInstallation
    {

        public String SourceFilepath { get; }
        public String UnpackDirectoryName { get; private set; }

        #region NotifiableProperties

        public new ZipInstallationStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        public ModCollection? Result { get; set; }
        public ModInstallationOptions Options { get; init; }

        private ZipInstallationStatus _status = ZipInstallationStatus.NotStarted;

        #endregion
        internal ZipInstallation(String source_file_path, ModInstallationOptions options)
        {
            SourceFilepath = source_file_path;

            Options = options;

            HeaderText = TextManager.Instance.GetText("INSTALLATION_HEADER_MOD");
            AdditionalText = new SimpleText(SourceFilepath);
        }

        public override Task<IInstallation> Setup()
        {
            IsInstalling = true;
            Status = ZipInstallationStatus.Unpacking;

            return Task.Run(async () =>
            {
                Console.WriteLine($"Extract zip: {SourceFilepath}");
                Result = await ModCollectionLoader.ExtractZipAsync(SourceFilepath,
                    Options.UnpackDirectory,
                    this);
                return this as IInstallation;
            }
            );
        }

        public override Task Finalize()
        {
            return this.RunMoveInto();
        }

        public override void CleanUp()
        { 
        
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
