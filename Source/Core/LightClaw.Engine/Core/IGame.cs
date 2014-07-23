using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Input;
using OpenTK.Platform;

namespace LightClaw.Engine.Core
{
    public interface IGame : IDisposable, INameable
    {
        Assembly GameCodeAssembly { get; }

        IGameWindow GameWindow { get; }

        ISceneManager SceneManager { get; }

        void Run();
    }
}
