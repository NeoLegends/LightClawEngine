using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    public interface IGame : IDisposable
    {
        event EventHandler<SceneLoadEventArgs> SceneLoading;

        event EventHandler<SceneLoadEventArgs> SceneLoaded;

        Scene Scene { get; }

        void Run();
    }
}
