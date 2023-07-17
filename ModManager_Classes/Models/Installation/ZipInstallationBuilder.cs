using Imya.Models.Installation.Interfaces;
using Imya.Services;
using Imya.Services.Interfaces;
using Imya.Texts;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.Installation
{
    public class ZipInstallationBuilderFactory : IZipInstallationBuilderFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ZipInstallationBuilderFactory(IServiceProvider serviceProvider) 
        {
            _serviceProvider = serviceProvider;
        }
        public ZipInstallationBuilder Create() => _serviceProvider.GetRequiredService<ZipInstallationBuilder>();
    }

    public class ZipInstallationBuilder
    {
        private String? _source;

        private readonly IGameSetupService _gameSetupService;
        private readonly IImyaSetupService _imyaSetupService;
        private readonly ITextManager _textManager;

        public ZipInstallationBuilder(
            IGameSetupService gameSetupService,
            IImyaSetupService imyaSetupService, 
            ITextManager textManager)
        {
            _gameSetupService = gameSetupService;
            _imyaSetupService = imyaSetupService;
            _textManager = textManager;
        }

        public ZipInstallationBuilder WithSource(String source_path) 
        {
            _source = source_path;
            return this;
        }

        public ZipInstallation Build()
        {
            if (_gameSetupService.GameRootPath is null)
                throw new Exception("No Game Path set!");

            if(_source is null)
                throw new Exception("Please set a source path before building!");

            var header = Path.GetFileName(_source);

            var guid = Guid.NewGuid().ToString();
            var installation = new ZipInstallation()
            {
                SourceFilepath = _source,
                UnpackTargetPath = Path.Combine(_imyaSetupService.UnpackDirectoryPath, guid),
                Status = InstallationStatus.NotStarted,
                CancellationTokenSource = new CancellationTokenSource(),
                HeaderText = _textManager.GetText("INSTALLATION_HEADER_MOD"),
                AdditionalText = new SimpleText(_source)
            };
            return installation;
        }
    }
}
