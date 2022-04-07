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

        #region NotifiableProperties
        
        public bool AllowOldToOverwrite
        {
            get => _allowOldToOverwrite;
            set => SetProperty(ref _allowOldToOverwrite, value);
        }
        private bool _allowOldToOverwrite = false;

        private ModCollection? result;

        #endregion
        public ZipInstallation(String source_file_path, String download_directory_name)
        {
            SourceFilepath = source_file_path;
            UnpackDirectoryName = download_directory_name;
        }

        public Task<ZipInstallation> RunUnpack()
        {
            IsInstalling = true;
            var allowOldToOverwrite = AllowOldToOverwrite;

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

            Console.WriteLine($"Install zip: {result?.ModsPath}");

            return Task.Run(async () =>
            {
                try
                {
                    await ModCollection.Global?.MoveIntoAsync(result, AllowOldToOverwrite);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Move Error: {e.Message}");
                }
            }
            );
        }

        public override string ToString() => $"InstallationTask of {SourceFilepath}";
    }

}
