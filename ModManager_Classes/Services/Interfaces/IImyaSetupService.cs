using Imya.Models.Mods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Services.Interfaces
{
    public interface IImyaSetupService
    {
        public String WorkingDirectoryPath { get; }
        public String ProfilesDirectoryPath { get; }
        public String DownloadDirectoryPath { get; }
        public String TweakDirectoryPath { get; }
        public String UnpackDirectoryPath { get; }

        void Init();
    }
}
