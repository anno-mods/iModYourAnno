using Imya.Models.NotifyPropertyChanged;
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
    public class ZipInstallation : Installation
    {
        public String SourceFilepath { get; }
        public String UnpackDirectoryName { get; private set; }


        public ZipInstallationOptions Options { get; }

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
        private ZipInstallationStatus _status = ZipInstallationStatus.NotStarted;

        private ModCollection? result;

        #endregion
        public ZipInstallation(String source_file_path, String download_directory_name, ZipInstallationOptions options)
        {
            SourceFilepath = source_file_path;
            UnpackDirectoryName = download_directory_name;

            Options = options;
        }

        public Task<ZipInstallation> RunUnpack()
        {
            IsInstalling = true;
            Status = ZipInstallationStatus.Unpacking;

            return Task.Run(async () =>
            {
                Console.WriteLine($"Extract zip: {SourceFilepath}");
                result = await ModInstaller.ExtractZipAsync(SourceFilepath,
                    Path.Combine(Directory.GetCurrentDirectory(), UnpackDirectoryName),
                    this);
                return this;
            }
            );
        }

        public Task RunMove()
        {
            if (result is null)
            {
                Console.WriteLine("No ModCollection to install");
                return Task.CompletedTask;
            }

            Status = ZipInstallationStatus.MovingFiles;

            Console.WriteLine($"Install zip: {result?.ModsPath}");

            return Task.Run(async () =>
            {
                await ModCollection.Global?.MoveIntoAsync(result, Options.AllowOldToOverwrite);
            }
            );
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

    public class ZipInstallationOptions : PropertyChangedNotifier
    {
        public bool AllowOldToOverwrite
        {
            get => _allowOldToOverwrite;
            set {
                _allowOldToOverwrite = value;
                OnPropertyChanged(nameof(AllowOldToOverwrite));
            }   
        }
        private bool _allowOldToOverwrite = false;
    }

}
