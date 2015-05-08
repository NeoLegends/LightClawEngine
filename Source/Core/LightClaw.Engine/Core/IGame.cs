using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Threading;
using OpenTK.Platform;

namespace LightClaw.Engine.Core
{
    /// <summary>
    /// Represents a <see cref="Game"/>.
    /// </summary>
    public interface IGame : IDispatcherObject, IDisposable, INameable
    {
        /// <summary>
        /// The current game time.
        /// </summary>
        GameTime CurrentGameTime { get; }

        /// <summary>
        /// The <see cref="IGameWindow"/> the game is shown in.
        /// </summary>
        IGameWindow GameWindow { get; }

        /// <summary>
        /// The <see cref="ISceneManager"/> managing the currently running <see cref="Scene"/>s.
        /// </summary>
        ISceneManager SceneManager { get; }

        /// <summary>
        /// Runs the game.
        /// </summary>
        void Run();
    }
}
