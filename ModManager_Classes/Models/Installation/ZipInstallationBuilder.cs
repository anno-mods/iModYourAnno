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

            var installation = new ZipInstallation();

            var guid = Guid.NewGuid().ToString();
            installation.SourceFilepath = _source;
            installation.UnpackTargetPath = Path.Combine(ImyaSetupManager.Instance.UnpackDirectoryPath, guid);
            return installation;
        }
    }
}
