using Imya.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.GameLauncher
{
    /* Standard Game Launch:
     * Start startprocess
     * startprocess calls ubiconnect
     * startprocess gets killed
     * ubiconnect starts the game
     */
    public class StandardGameLauncher : GameLauncherBase, IGameLauncher
    {
        private GameSetupManager _gameSetup; 

        public StandardGameLauncher()
        {
            _scanner = new GameScanner();
            _gameSetup = GameSetupManager.Instance;

            RunningGame = null; 
        }

        public void StartGame()
        {
            _ = Task.Run(async () => await LaunchUbiAsync());
        }

        private async Task LaunchUbiAsync()
        {
            using Process process = new Process();
            process.StartInfo.FileName = _gameSetup.ExecutablePath;
            process.Start();

            Console.WriteLine("Anno 1800 started.");
            OnGameStarted();
            await process.WaitForExitAsync();

            await ScanGameAsync(); 
        }
    }
}
