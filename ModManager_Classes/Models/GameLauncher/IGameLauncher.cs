using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.GameLauncher
{
    public interface IGameLauncher : IDisposable
    {
        Process? RunningGame { get; }
        bool HasRunningGame { get => RunningGame is not null; }

        void StartGame();

        delegate void GameStartEventHandler();
        event GameStartEventHandler GameStarted;

        delegate void GameCloseEventHandler(int GameExitCode, bool IsRegularExit = true);
        event GameCloseEventHandler GameExited;
    }
}
