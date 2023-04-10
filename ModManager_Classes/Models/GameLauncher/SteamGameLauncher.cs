using Imya.Services;
using Imya.Services.Interfaces;
using Imya.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.GameLauncher
{
    /* Steam Game Launch: 
     * 
     * start startprocess
     * startprocess calls steam
     * startprocess gets killed
     * steam starts ubisoft_process
     * ubisoft_process does shit and then also gets killed
     * ubisoft starts game.
     */
    public class SteamGameLauncher : GameLauncherBase, IGameLauncher
    {
        private IGameSetupService _gameSetup;

        public SteamGameLauncher(IGameSetupService gameSetupService)
        {
            _scanner = new GameScanner();
            _gameSetup = gameSetupService;

            RunningGame = null;
        }

        public void StartGame()
        {
            _ = Task.Run(async () =>
            {
                await LaunchSteamAsync();
                await ScanUbiAsync();
                await ScanGameAsync(); 
            });
        }

        private async Task LaunchSteamAsync()
        {
            var ps = new ProcessStartInfo("steam://rungameid/916440")
            {
                UseShellExecute = true,
                Verb = "open"
            };
            var game = Process.Start(ps);

            if (game is null)
            {
                OnGameExited(-1, false);
                return;
            }

            Console.WriteLine("Anno 1800 started.");
            OnGameStarted(); 
            game.EnableRaisingEvents = true;
            await game.WaitForExitAsync();
        }

        private async Task ScanUbiAsync()
        {
            var ubiprocess = await _scanner.ScanForRunningGameAsync(2);
            await ubiprocess.WaitForExitAsync();
        }
    }
}
