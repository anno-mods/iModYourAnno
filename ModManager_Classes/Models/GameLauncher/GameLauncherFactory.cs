using Imya.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.GameLauncher
{
    public class GameLauncherFactory : IGameLauncherFactory
    {
        GameSetupManager _gameSetup; 

        public GameLauncherFactory() 
        {
            _gameSetup = GameSetupManager.Instance; 
        }

        public IGameLauncher GetLauncher()
        {
            return _gameSetup.GamePlatform == GamePlatform.Steam ? new SteamGameLauncher() : new StandardGameLauncher();
        }
    }
}
