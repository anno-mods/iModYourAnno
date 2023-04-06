using Imya.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.GameLauncher
{
    public abstract class GameLauncherBase : IDisposable
    {
        public event IGameLauncher.GameStartEventHandler GameStarted = delegate { };
        public event IGameLauncher.GameCloseEventHandler GameExited = delegate { };

        public Process? RunningGame { get; protected set; }
        protected GameScanner _scanner;

        public GameLauncherBase() {
            _scanner = new GameScanner();
            RunningGame = null;
        }

        protected async Task ScanGameAsync()
        {
            Console.WriteLine($"Start Process exited! Starting Game Scan");
            RunningGame = await _scanner.ScanForRunningGameAsync(30);

            if (RunningGame is null)
            {
                OnGameExited(-1, false);
                return;
            }
            RunningGame.EnableRaisingEvents = true;
            RunningGame.Exited += OnGameExit;
            await RunningGame.WaitForExitAsync();
        }

        protected void OnGameExit(object sender, EventArgs e)
        {
            var process = sender as Process;
            Console.WriteLine($"Anno 1800 exited with Code {process?.ExitCode}");
            int exitCode = process?.ExitCode ?? -1;
            OnGameExited(exitCode);
            RunningGame = null;
        }

        protected void OnGameExited(int exitCode, bool IsRegularExit = true) => GameExited.Invoke(exitCode, IsRegularExit);

        protected void OnGameStarted() => GameStarted.Invoke();

        public void Dispose()
        {
            foreach (Delegate d in GameExited.GetInvocationList())
                GameExited -= (IGameLauncher.GameCloseEventHandler)d;
            foreach (Delegate d in GameStarted.GetInvocationList())
                GameStarted -= (IGameLauncher.GameStartEventHandler)d;
        }
    }
}
