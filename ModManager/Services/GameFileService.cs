using Imya.Models;
using Imya.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Pfim;
using AnnoRDA;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using AnnoRDA.Builder;

namespace Imya.Services
{
    public class GameFileService : IGameFilesService
    {
        private IGameSetupService _gameSetupService;

        private FileSystem _fileSystem;

        private bool isLoading = false; 

        public GameFileService(IGameSetupService gameSetupService)
        {
            _gameSetupService = gameSetupService;
            _gameSetupService.GameRootPathChanged += async (newpath) => await LoadAsync();  
        }

        public async Task LoadAsync()
        {
            if (!_gameSetupService.IsMaindataValid)
                return;
            isLoading = true;
            await Task.Run(() =>
            {
                var fs = FileSystemBuilder.Create()
                    .FromPath(_gameSetupService.MaindataPath)
                    .WithDefaultSorting()
                    .OnlyArchivesMatchingWildcard(@"data*.rda")
                    .AddWhitelisted("*.png", "*_0.dds")
                    .Build();
                _fileSystem = fs;
                isLoading = false;
            });
        }

        public Stream? OpenFile(string path)
        {
            if (isLoading)
                return null;

            return _fileSystem.OpenRead(path);
        }

        /// <summary>
        /// Like OpenFile, but redirects *.png to *_0.dds
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Stream? OpenIcon(string path) 
        {
            if (path.EndsWith(".png"))
                path = path[0..^4] + "_0.dds";
            return OpenFile(path);
        }
    }
}
