using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    public interface IControllable : IDisposable
    {
        event EventHandler<ControllableEventArgs> Loaded;

        event EventHandler<ControllableEventArgs> Updated;

        event EventHandler<ControllableEventArgs> Unloaded;

        bool IsLoaded { get; }

        void Load();

        void Update();

        void Unload();
    }
}
