using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    public interface IControllable
    {
        event EventHandler<ControllableEventArgs> Loaded;

        event EventHandler<ControllableEventArgs> Updated;

        event EventHandler<ControllableEventArgs> ShutDown;

        bool IsLoaded { get; }

        Task Load();

        Task Update();

        Task Shutdown();
    }
}
