using Anno.EasyMod.Mods;

namespace Imya.Services.Interfaces
{
    public interface IImyaSetupService
    {
        public String WorkingDirectoryPath { get; }
        public String ProfilesDirectoryPath { get; }
        public String DownloadDirectoryPath { get; }
        public String TweakDirectoryPath { get; }
        public String UnpackDirectoryPath { get; }

        public IModCollection GlobalModCollection { get; set; }

        void Init();
    }
}
