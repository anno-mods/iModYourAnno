using Imya.Services;
using Imya.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.GameLauncher
{
    public class GameLauncherFactory : IGameLauncherFactory
    {
        IGameSetupService _gameSetupService;
        StandardGameLauncher _standardGameLauncher;
        SteamGameLauncher _steamGameLauncher; 

        public GameLauncherFactory(
            IGameSetupService gameSetupService,
            StandardGameLauncher stdGameLauncher,
            SteamGameLauncher steamGameLauncher) 
        {
            _standardGameLauncher = stdGameLauncher;
            _steamGameLauncher = steamGameLauncher;
            _gameSetupService = gameSetupService; 
        }

        public IGameLauncher GetLauncher()
        {
            return _gameSetupService.GamePlatform ==
                GamePlatform.Steam ?
                    _steamGameLauncher
                    : _standardGameLauncher;
        }
    }
}
