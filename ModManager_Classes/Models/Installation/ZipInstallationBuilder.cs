using Imya.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.Installation
{
    public class ZipInstallationBuilder
    {
        private String? _source;

        private ZipInstallationBuilder() { }

        public static ZipInstallationBuilder Create() => new ZipInstallationBuilder();

        public ZipInstallationBuilder WithSource(String source_path) 
        {
            _source = source_path;
            return this;
        }

        public ZipInstallation Build()
        {
            if (GameSetupManager.Instance.GameRootPath is null)
                throw new Exception("No Game Path set!");

            if(_source is null)
                throw new Exception("Please set a source path before building!");

            var header = Path.GetFileName(_source);

            var guid = Guid.NewGuid().ToString();
            var installation = new ZipInstallation()
            {
                SourceFilepath = _source,
                UnpackTargetPath = Path.Combine(ImyaSetupManager.Instance.UnpackDirectoryPath, guid),
                HeaderText = new SimpleText(header),
                Status = InstallationStatus.NotStarted
            };
            return installation;
        }
    }
}
